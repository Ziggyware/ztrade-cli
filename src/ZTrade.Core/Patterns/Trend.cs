using ZTrade.Core.Market;

namespace ZTrade.Core.Patterns;

public enum TrendDirection { None, Up, Down }

public static class Trend
{
    /// <summary>
    /// Direction of the least-squares slope of closing prices over the window.
    /// Returns None for fewer than 2 candles or a zero slope.
    /// </summary>
    public static TrendDirection Direction(IReadOnlyList<Candle> candles, int start, int count)
    {
        ArgumentNullException.ThrowIfNull(candles);
        if (count < 2 || start < 0 || start + count > candles.Count) return TrendDirection.None;

        // slope numerator = sum((x - xMean) * (y - yMean)); denominator is always > 0 for count >= 2.
        var xMean = (count - 1) / 2m;
        decimal yMean = 0m;
        for (var i = 0; i < count; i++) yMean += candles[start + i].Close;
        yMean /= count;

        decimal numerator = 0m;
        for (var i = 0; i < count; i++) numerator += (i - xMean) * (candles[start + i].Close - yMean);

        return numerator > 0m ? TrendDirection.Up : numerator < 0m ? TrendDirection.Down : TrendDirection.None;
    }
}
