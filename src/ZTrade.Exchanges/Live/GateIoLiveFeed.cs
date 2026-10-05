using System.Globalization;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.Live;

/// <summary>Gate.io v4 public spot trades and ticker stream. This adapter never authenticates.</summary>
public sealed partial class GateIoLiveFeed : ILiveMarketFeed
{
    private static readonly Uri Endpoint = new("wss://api.gateio.ws/ws/v4/");

    public string ExchangeName => "Gate.io";

    public async IAsyncEnumerable<LiveFeedEvent> StreamAsync(
        string symbol,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeSymbol(symbol);
        using var socket = new ClientWebSocket();
        socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
        await socket.ConnectAsync(Endpoint, cancellationToken).ConfigureAwait(false);

        await WebSocketStream.SendJsonAsync(socket, Subscription("spot.trades", normalized, 1), cancellationToken).ConfigureAwait(false);
        await WebSocketStream.SendJsonAsync(socket, Subscription("spot.tickers", normalized, 2), cancellationToken).ConfigureAwait(false);
        using var heartbeatCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var heartbeatTask = SendHeartbeatsAsync(socket, heartbeatCancellation.Token);
        try
        {
            yield return new LiveFeedConnection(true, "Gate.io market-data socket connected.");
            await foreach (var payload in WebSocketStream.ReadTextMessagesAsync(socket, cancellationToken).ConfigureAwait(false))
            {
                foreach (var update in GateIoLiveStreamParser.Parse(payload, normalized)) yield return update;
            }
        }
        finally
        {
            heartbeatCancellation.Cancel();
            try { await heartbeatTask.ConfigureAwait(false); }
            catch (OperationCanceledException) when (heartbeatCancellation.IsCancellationRequested) { }
        }
    }

    private static async Task SendHeartbeatsAsync(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            var ping = new { time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), channel = "spot.ping" };
            await WebSocketStream.SendJsonAsync(socket, ping, cancellationToken).ConfigureAwait(false);
        }
    }

    internal static string NormalizeSymbol(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        var normalized = symbol.Trim().ToUpperInvariant();
        if (!SymbolPattern().IsMatch(normalized))
        {
            throw new ArgumentException($"Gate.io symbol '{symbol}' must look like BASE_QUOTE, e.g. BTC_USDT.", nameof(symbol));
        }

        return normalized;
    }

    private static object Subscription(string channel, string symbol, int id) => new
    {
        time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        channel,
        @event = "subscribe",
        payload = new[] { symbol },
        id,
    };

    [GeneratedRegex("^[A-Z0-9]{1,20}_[A-Z0-9]{1,20}$", RegexOptions.CultureInvariant)]
    private static partial Regex SymbolPattern();
}

/// <summary>Parser for Gate.io v4 <c>spot.trades</c> and <c>spot.tickers</c> updates.</summary>
public static class GateIoLiveStreamParser
{
    public static IReadOnlyList<LiveFeedEvent> Parse(ReadOnlyMemory<byte> utf8Json, string expectedSymbol)
    {
        using var document = JsonDocument.Parse(utf8Json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object
            || !EqualsText(root, "event", "update")
            || !root.TryGetProperty("channel", out _))
        {
            return Array.Empty<LiveFeedEvent>();
        }

        var channel = Text(root, "channel");
        var rootTimestamp = Long(root, "time_ms") ?? SecondsToMilliseconds(Long(root, "time"))
            ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (!root.TryGetProperty("result", out var result)) return Array.Empty<LiveFeedEvent>();

        var updates = new List<LiveFeedEvent>();
        if (string.Equals(channel, "spot.trades", StringComparison.Ordinal))
        {
            // Gate.io v4 sends one trade object in result. Accept arrays too for compatible batch payloads.
            if (result.ValueKind == JsonValueKind.Object)
            {
                AddTrade(result, expectedSymbol, rootTimestamp, updates);
            }
            else if (result.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in result.EnumerateArray()) AddTrade(item, expectedSymbol, rootTimestamp, updates);
            }
        }
        else if (string.Equals(channel, "spot.tickers", StringComparison.Ordinal))
        {
            if (result.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in result.EnumerateArray()) AddTicker(item, expectedSymbol, rootTimestamp, updates);
            }
            else if (result.ValueKind == JsonValueKind.Object)
            {
                AddTicker(result, expectedSymbol, rootTimestamp, updates);
            }
        }

        return updates;
    }

    private static void AddTrade(JsonElement item, string expectedSymbol, long rootTimestamp, ICollection<LiveFeedEvent> updates)
    {
        if (item.ValueKind != JsonValueKind.Object
            || !item.TryGetProperty("price", out _)
            || !item.TryGetProperty("amount", out _)) return;
        var itemSymbol = Text(item, "currency_pair");
        if (itemSymbol is not null && !string.Equals(itemSymbol, expectedSymbol, StringComparison.OrdinalIgnoreCase)) return;
        var price = Decimal(item, "price");
        var quantity = Decimal(item, "amount");
        if (price is null || quantity is null || price <= 0m || quantity < 0m) return;
        var timestamp = Milliseconds(item, "create_time_ms")
            ?? SecondsToMilliseconds(Long(item, "create_time"))
            ?? rootTimestamp;
        if (timestamp < 0) return;
        var side = Text(item, "side")?.ToLowerInvariant() switch
        {
            "buy" => TradeSide.Buy,
            "sell" => TradeSide.Sell,
            _ => TradeSide.Unknown,
        };
        updates.Add(new LiveFeedTrade(new MarketTrade(
            timestamp, price.Value, quantity.Value, side, Text(item, "id"))));
    }

    private static void AddTicker(JsonElement item, string expectedSymbol, long timestamp, ICollection<LiveFeedEvent> updates)
    {
        if (item.ValueKind != JsonValueKind.Object) return;
        var symbol = Text(item, "currency_pair");
        if (symbol is not null && !string.Equals(symbol, expectedSymbol, StringComparison.OrdinalIgnoreCase)) return;
        var bid = Decimal(item, "highest_bid");
        var ask = Decimal(item, "lowest_ask");
        if (bid is <= 0m) bid = null;
        if (ask is <= 0m) ask = null;
        var quoteTimestamp = Long(item, "time_ms") ?? SecondsToMilliseconds(Long(item, "time")) ?? timestamp;
        if (quoteTimestamp < 0) return;
        updates.Add(new LiveFeedQuote(new MarketQuote(
            quoteTimestamp,
            bid,
            null,
            ask,
            null,
            Decimal(item, "change_percentage"),
            Decimal(item, "base_volume"))));
    }

    private static bool EqualsText(JsonElement element, string property, string expected) =>
        string.Equals(Text(element, property), expected, StringComparison.Ordinal);

    private static string? Text(JsonElement element, string name)
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

    private static long? Milliseconds(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property)) return null;
        var text = property.ValueKind == JsonValueKind.String ? property.GetString() : property.GetRawText();
        if (!decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            || value < 0m || value > long.MaxValue) return null;
        return decimal.ToInt64(decimal.Truncate(value));
    }

    private static long? SecondsToMilliseconds(long? seconds) =>
        seconds is { } value && value <= long.MaxValue / 1000 && value >= 0 ? value * 1000 : null;

    private static decimal? Decimal(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property)) return null;
        var text = property.ValueKind == JsonValueKind.String ? property.GetString() : property.GetRawText();
        return decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : null;
    }
}
