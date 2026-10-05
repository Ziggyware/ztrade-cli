using ZTrade.Core.Market;

namespace ZTrade.Exchanges;

/// <summary>Read-only market data. Deliberately has no order or account methods.</summary>
public interface IMarketDataClient
{
    /// <summary>Returns up to <paramref name="count"/> most recent CLOSED candles, ascending by time.</summary>
    Task<IReadOnlyList<Candle>> GetRecentCandlesAsync(
        string symbol, CandleInterval interval, int count, CancellationToken cancellationToken = default);
}

public sealed class MarketDataException : Exception
{
    public MarketDataException(string message, int? statusCode = null, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

    public int? StatusCode { get; }
}
