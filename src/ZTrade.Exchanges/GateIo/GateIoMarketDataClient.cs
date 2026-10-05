using System.Globalization;
using System.Text.RegularExpressions;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.GateIo;

public sealed record GateIoClientOptions
{
    /// <summary>Must end with a slash so relative URIs resolve under /api/v4/.</summary>
    public Uri BaseAddress { get; init; } = new("https://api.gateio.ws/api/v4/");

    public int MaxAttempts { get; init; } = 4;
    public TimeSpan BaseRetryDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    public bool IncludeUnclosedCandles { get; init; }
}

/// <summary>
/// Public (unauthenticated) Gate.io spot candlestick client. Retries 429/5xx/network errors with
/// exponential backoff + jitter (honouring Retry-After). Takes an externally owned <see cref="HttpClient"/>.
/// </summary>
public sealed partial class GateIoMarketDataClient : IMarketDataClient
{
    public const int MaxPointsPerRequest = 1000;
    public const int MaxHistoryPoints = 10_000;

    private readonly HttpClient _http;
    private readonly GateIoClientOptions _options;
    private readonly TimeProvider _time;

    public GateIoMarketDataClient(HttpClient http, GateIoClientOptions? options = null, TimeProvider? timeProvider = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _options = options ?? new GateIoClientOptions();
        _time = timeProvider ?? TimeProvider.System;
        if (_options.MaxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(options), "MaxAttempts must be >= 1.");
        _http.BaseAddress ??= _options.BaseAddress;
    }

    public async Task<IReadOnlyList<Candle>> GetRecentCandlesAsync(
        string symbol, CandleInterval interval, int count, CancellationToken cancellationToken = default)
    {
        symbol = NormalizeSymbol(symbol);
        if (count < 1 || count > MaxHistoryPoints)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"count must be within 1..{MaxHistoryPoints}.");
        }

        var intervalText = interval.ToShortString();

        if (count <= MaxPointsPerRequest)
        {
            var body = await GetAsync(
                $"spot/candlesticks?currency_pair={symbol}&interval={intervalText}&limit={count}",
                cancellationToken).ConfigureAwait(false);
            return GateIoCandleParser.Parse(body, _options.IncludeUnclosedCandles);
        }

        if (interval == CandleInterval.Day30)
        {
            // Gate.io defines 30d as a calendar month, so fixed-width window arithmetic would be wrong.
            throw new NotSupportedException("Paged history is not supported for the 30d interval; use count <= 1000.");
        }

        var step = (long)interval.Seconds();
        var now = _time.GetUtcNow().ToUnixTimeSeconds();
        var alignedNow = now - now % step;
        var firstStart = alignedNow - (count - 1) * step;

        var byTimestamp = new SortedDictionary<long, Candle>();
        for (var windowStart = firstStart; windowStart <= alignedNow; windowStart += MaxPointsPerRequest * step)
        {
            var windowEnd = Math.Min(windowStart + (MaxPointsPerRequest - 1) * step, alignedNow);
            var body = await GetAsync(
                string.Create(CultureInfo.InvariantCulture,
                    $"spot/candlesticks?currency_pair={symbol}&interval={intervalText}&from={windowStart}&to={windowEnd}"),
                cancellationToken).ConfigureAwait(false);

            foreach (var candle in GateIoCandleParser.Parse(body, _options.IncludeUnclosedCandles))
            {
                byTimestamp[candle.Timestamp] = candle;
            }
        }

        return byTimestamp.Values.ToArray();
    }

    internal static string NormalizeSymbol(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        var normalized = symbol.Trim().ToUpperInvariant();
        if (!SymbolPattern().IsMatch(normalized))
        {
            throw new ArgumentException($"Symbol '{symbol}' must look like BASE_QUOTE, e.g. BTC_USDT.", nameof(symbol));
        }

        return normalized;
    }

    [GeneratedRegex("^[A-Z0-9]{1,20}_[A-Z0-9]{1,20}$", RegexOptions.CultureInvariant)]
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
                last = new MarketDataException($"Gate.io returned HTTP {status}: {Truncate(body)}", status);
                if (status != 429 && status < 500) throw last; // 4xx other than 429 is not retryable
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
