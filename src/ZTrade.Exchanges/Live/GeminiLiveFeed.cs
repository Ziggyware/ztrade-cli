using System.Globalization;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.Live;

/// <summary>Gemini public market-data WebSocket: individual prints plus the live top of book.</summary>
public sealed partial class GeminiLiveFeed : ILiveMarketFeed
{
    private static readonly Uri WebSocketBase = new("wss://api.gemini.com/");

    public string ExchangeName => "Gemini";

    public async IAsyncEnumerable<LiveFeedEvent> StreamAsync(
        string symbol,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeSymbol(symbol);
        var uri = new Uri(WebSocketBase,
            $"v1/marketdata/{normalized}?heartbeat=true&top_of_book=true&bids=true&offers=true");
        using var socket = new ClientWebSocket();
        socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
        await socket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        yield return new LiveFeedConnection(true, "Gemini market-data socket connected.");

        var parser = new GeminiLiveStreamParser();
        await foreach (var payload in WebSocketStream.ReadTextMessagesAsync(socket, cancellationToken).ConfigureAwait(false))
        {
            foreach (var update in parser.Parse(payload)) yield return update;
        }
    }

    internal static string NormalizeSymbol(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        var normalized = symbol.Trim().ToUpperInvariant();
        if (!SymbolPattern().IsMatch(normalized))
        {
            throw new ArgumentException($"Gemini symbol '{symbol}' must contain 3–20 letters or digits, e.g. BTCUSD.", nameof(symbol));
        }

        return normalized;
    }

    [GeneratedRegex("^[A-Z0-9]{3,20}$", RegexOptions.CultureInvariant)]
    private static partial Regex SymbolPattern();
}

/// <summary>Stateful parser so Gemini book-change messages can maintain an exact best bid and ask.</summary>
public sealed class GeminiLiveStreamParser
{
    private readonly SortedDictionary<decimal, decimal> _bids = new();
    private readonly SortedDictionary<decimal, decimal> _asks = new();

    public IReadOnlyList<LiveFeedEvent> Parse(ReadOnlyMemory<byte> utf8Json)
    {
        using var document = JsonDocument.Parse(utf8Json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty("type", out var rootType)
            || rootType.ValueKind != JsonValueKind.String
            || !string.Equals(rootType.GetString(), "update", StringComparison.Ordinal))
        {
            return Array.Empty<LiveFeedEvent>();
        }

        var rootTimestamp = Long(root, "timestampms") ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (!root.TryGetProperty("events", out var events) || events.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LiveFeedEvent>();
        }

        List<LiveFeedEvent>? updates = null;
        foreach (var item in events.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("type", out var typeElement)) continue;
            var type = typeElement.ValueKind == JsonValueKind.String ? typeElement.GetString() : null;
            var timestamp = Long(item, "timestampms") ?? rootTimestamp;
            if (timestamp < 0) continue;
            if (string.Equals(type, "trade", StringComparison.Ordinal))
            {
                var price = Decimal(item, "price");
                var quantity = Decimal(item, "amount");
                if (price is null || quantity is null || price <= 0m || quantity < 0m) continue;
                var makerSide = String(item, "makerSide");
                var aggressorSide = makerSide switch
                {
                    "bid" => TradeSide.Sell,
                    "ask" => TradeSide.Buy,
                    _ => TradeSide.Unknown,
                };
                updates ??= new List<LiveFeedEvent>();
                updates.Add(new LiveFeedTrade(new MarketTrade(
                    timestamp,
                    price.Value,
                    quantity.Value,
                    aggressorSide,
                    String(item, "tid"))));
                continue;
            }

            if (!string.Equals(type, "change", StringComparison.Ordinal)) continue;
            var side = String(item, "side");
            var bookPrice = Decimal(item, "price");
            var remaining = Decimal(item, "remaining");
            if (bookPrice is null || remaining is null || bookPrice <= 0m || remaining < 0m) continue;
            var book = side switch
            {
                "bid" => _bids,
                "ask" => _asks,
                _ => null,
            };
            if (book is null) continue;
            if (remaining == 0m) book.Remove(bookPrice.Value);
            else book[bookPrice.Value] = remaining.Value;

            KeyValuePair<decimal, decimal>? bestBid = _bids.Count == 0 ? null : _bids.Last();
            KeyValuePair<decimal, decimal>? bestAsk = _asks.Count == 0 ? null : _asks.First();
            updates ??= new List<LiveFeedEvent>();
            updates.Add(new LiveFeedQuote(new MarketQuote(
                timestamp,
                bestBid?.Key,
                bestBid?.Value,
                bestAsk?.Key,
                bestAsk?.Value)));
        }

        if (updates is null) return Array.Empty<LiveFeedEvent>();
        return updates;
    }

    private static string? String(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property)) return null;
        return property.ValueKind == JsonValueKind.String ? property.GetString() : property.GetRawText();
    }

    private static long? Long(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property)) return null;
        var text = property.ValueKind == JsonValueKind.String ? property.GetString() : property.GetRawText();
        return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    private static decimal? Decimal(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property)) return null;
        var text = property.ValueKind == JsonValueKind.String ? property.GetString() : property.GetRawText();
        return decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : null;
    }
}
