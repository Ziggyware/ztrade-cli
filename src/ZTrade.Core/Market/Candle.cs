namespace ZTrade.Core.Market;

/// <summary>
/// Immutable, validated OHLCV bar. <see cref="Timestamp"/> is the bar OPEN time, Unix seconds UTC.
/// Prices are <see cref="decimal"/> so arithmetic on exchange-quoted values is exact.
/// Note: <c>default(Candle)</c> bypasses validation (all zeros); never treat it as market data.
/// </summary>
public readonly record struct Candle
{
    public Candle(long timestamp, decimal open, decimal high, decimal low, decimal close, decimal volume = 0m)
    {
        if (timestamp < 0) throw new ArgumentOutOfRangeException(nameof(timestamp), "Timestamp must be >= 0.");
        if (open <= 0m) throw new ArgumentOutOfRangeException(nameof(open), "Prices must be > 0.");
        if (high <= 0m) throw new ArgumentOutOfRangeException(nameof(high), "Prices must be > 0.");
        if (low <= 0m) throw new ArgumentOutOfRangeException(nameof(low), "Prices must be > 0.");
        if (close <= 0m) throw new ArgumentOutOfRangeException(nameof(close), "Prices must be > 0.");
        if (volume < 0m) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be >= 0.");
        if (high < Math.Max(open, close) || low > Math.Min(open, close) || high < low)
        {
            throw new ArgumentException("OHLC invariant violated: low <= min(open, close) <= max(open, close) <= high.");
        }

        Timestamp = timestamp;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    public long Timestamp { get; }
    public decimal Open { get; }
    public decimal High { get; }
    public decimal Low { get; }
    public decimal Close { get; }

    /// <summary>Volume as reported by the exchange.</summary>
    public decimal Volume { get; }

    public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(Timestamp);
    public bool IsBullish => Close > Open;
    public bool IsBearish => Close < Open;
    public decimal BodyHigh => Math.Max(Open, Close);
    public decimal BodyLow => Math.Min(Open, Close);
    public decimal Body => BodyHigh - BodyLow;
    public decimal Range => High - Low;
    public decimal UpperShadow => High - BodyHigh;
    public decimal LowerShadow => BodyLow - Low;
    public decimal BodyMidpoint => (Open + Close) / 2m;

    public override string ToString() =>
        $"{Time:yyyy-MM-dd HH:mm}Z O={Open} H={High} L={Low} C={Close} V={Volume}";
}
