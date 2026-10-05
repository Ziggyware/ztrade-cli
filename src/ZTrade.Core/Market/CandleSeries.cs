namespace ZTrade.Core.Market;

public static class CandleSeries
{
    /// <summary>Throws unless timestamps are strictly ascending (required by indicators, patterns, backtests).</summary>
    public static void EnsureAscending(IReadOnlyList<Candle> candles)
    {
        ArgumentNullException.ThrowIfNull(candles);
        for (var i = 1; i < candles.Count; i++)
        {
            if (candles[i].Timestamp <= candles[i - 1].Timestamp)
            {
                throw new ArgumentException(
                    $"Candles must be strictly ascending by timestamp; violation at index {i}.", nameof(candles));
            }
        }
    }
}
