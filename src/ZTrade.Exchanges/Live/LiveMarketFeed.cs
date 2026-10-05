using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.Live;

public abstract record LiveFeedEvent;

public sealed record LiveFeedConnection(bool Connected, string? Detail = null) : LiveFeedEvent;

public sealed record LiveFeedTrade(MarketTrade Trade) : LiveFeedEvent;

public sealed record LiveFeedQuote(MarketQuote Quote) : LiveFeedEvent;

/// <summary>Unauthenticated, read-only trade and quote stream. There are deliberately no order APIs.</summary>
public interface ILiveMarketFeed
{
    string ExchangeName { get; }

    IAsyncEnumerable<LiveFeedEvent> StreamAsync(string symbol, CancellationToken cancellationToken = default);
}

/// <summary>Common fragmented-frame reader for the exchange's public WebSocket feeds.</summary>
internal static class WebSocketStream
{
    private const int ReceiveBufferSize = 16 * 1024;
    private const int MaxMessageBytes = 2 * 1024 * 1024;

    public static async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadTextMessagesAsync(
        ClientWebSocket socket,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var buffer = new byte[ReceiveBufferSize];
        using var message = new MemoryStream(ReceiveBufferSize);
        while (!cancellationToken.IsCancellationRequested)
        {
            message.SetLength(0);
            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    yield break;
                }

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    throw new WebSocketException("The exchange sent an unexpected binary market-data frame.");
                }

                if (message.Length + result.Count > MaxMessageBytes)
                {
                    throw new WebSocketException($"Market-data frame exceeded {MaxMessageBytes} bytes.");
                }

                message.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            // The owned array must remain stable while a parser runs. The exchange parsers consume the
            // bytes synchronously before the iterator resumes, so one compact copy is sufficient.
            yield return message.ToArray();
        }
    }

    public static async Task SendJsonAsync(ClientWebSocket socket, object payload, CancellationToken cancellationToken)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload);
        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
    }
}
