using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.Gemini;

public sealed record GeminiClientOptions
{
    public Uri BaseAddress { get; init; } = new("https://api.gemini.com/");

    public int MaxAttempts { get; init; } = 4;
    public TimeSpan BaseRetryDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    public bool IncludeUnclosedCandles { get; init; }
}

public sealed partial class GeminiMarketDataClient : IMarketDataClient
{
    private readonly HttpClient _http;
    private readonly GeminiClientOptions _options;
    private readonly TimeProvider _time;

    public GeminiMarketDataClient(HttpClient http, GeminiClientOptions? options = null, TimeProvider? timeProvider = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _options = options ?? new GeminiClientOptions();
        _time = timeProvider ?? TimeProvider.System;
        if (_options.MaxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(options), "MaxAttempts must be >= 1.");
        _http.BaseAddress ??= _options.BaseAddress;
    }

    public async Task<IReadOnlyList<Candle>> GetRecentCandlesAsync(
        string symbol, CandleInterval interval, int count, CancellationToken cancellationToken = default)
    {
        symbol = NormalizeSymbol(symbol);
        if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), "count must be >= 1.");
        var frame = TimeFrame(interval)
            ?? throw new NotSupportedException($"Gemini does not support interval {interval.ToShortString()}; use {string.Join(", ", Enum.GetValues<CandleInterval>().ToList().Select(x => TimeFrame(x)))}.");

        var body = await GetAsync($"v2/candles/{symbol}/{frame}", cancellationToken).ConfigureAwait(false);
        IEnumerable<Candle> candles = GeminiCandleParser.Parse(body);

        if (!_options.IncludeUnclosedCandles)
        {
            var now = _time.GetUtcNow().ToUnixTimeSeconds();
            var width = interval.Seconds();
            candles = candles.Where(c => c.Timestamp + width <= now);
        }

        return candles.TakeLast(count).ToArray();
    }

    internal static string? TimeFrame(CandleInterval interval) => interval switch
    {
        CandleInterval.Minute1 => "1m",
        CandleInterval.Minute5 => "5m",
        CandleInterval.Minute15 => "15m",
        CandleInterval.Minute30 => "30m",
        CandleInterval.Hour1 => "1hr",
        CandleInterval.Hour6 => "6hr",
        CandleInterval.Day1 => "1day",
        _ => null,
    };

    internal static string NormalizeSymbol(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        var normalized = symbol.Trim().ToUpperInvariant();
        if (!SymbolPattern().IsMatch(normalized))
        {
            throw new ArgumentException($"Symbol '{symbol}' must look like BTCUSD (letters and digits only).", nameof(symbol));
        }

        return normalized;
    }

    [GeneratedRegex("^[A-Z0-9]{3,20}$", RegexOptions.CultureInvariant)]
    private static partial Regex SymbolPattern();

    private async Task<string> GetAsync(string relativeUri, CancellationToken ct)
    {
        MarketDataException? last = null;
        for (var attempt = 1; attempt <= _options.MaxAttempts; attempt++)
        {
            TimeSpan? retryAfter = null;
            try
            {
                using var response = await _http.GetAsync(relativeUri, ct).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) return body;

                var status = (int)response.StatusCode;
                last = new MarketDataException($"Gemini returned HTTP {status}: {Truncate(body)}", status);
                if (status != 429 && status < 500) throw last;
                retryAfter = response.Headers.RetryAfter?.Delta;
            }
            catch (HttpRequestException ex)
            {
                last = new MarketDataException($"Network error: {ex.Message}", inner: ex);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                last = new MarketDataException("Request timed out.");
            }

            if (attempt < _options.MaxAttempts)
            {
                await Task.Delay(retryAfter ?? Backoff(attempt), _time, ct).ConfigureAwait(false);
            }
        }

        throw last ?? new MarketDataException("Request failed.");
    }

    private TimeSpan Backoff(int attempt)
    {
        var exponential = _options.BaseRetryDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
        var jitter = Random.Shared.NextDouble() * 0.25 * exponential;
        return TimeSpan.FromMilliseconds(exponential + jitter);
    }

    private static string Truncate(string text) => text.Length <= 200 ? text : text[..200] + "…";
}
