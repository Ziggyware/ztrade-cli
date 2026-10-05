namespace ZTrade.Core.Patterns;

/// <summary>Tunable thresholds. Defaults follow the original ZTrade values where it had them.</summary>
public sealed record PatternOptions
{
    /// <summary>Doji if Body &lt;= this * (upper + lower shadow). Original: 0.1.</summary>
    public decimal DojiBodyToShadows { get; init; } = 0.1m;

    /// <summary>Dragonfly/Gravestone: dominant shadow must be at least this fraction of the range.</summary>
    public decimal DojiLongShadowFraction { get; init; } = 0.6m;

    /// <summary>Dragonfly/Gravestone: opposite shadow must be at most this fraction of the range.</summary>
    public decimal DojiShortShadowFraction { get; init; } = 0.1m;

    /// <summary>Long-legged doji: both shadows at least this fraction of the range.</summary>
    public decimal LongLeggedMinShadowFraction { get; init; } = 0.3m;

    /// <summary>Hammer family: long shadow &gt;= this * body. Original: 2.</summary>
    public decimal HammerShadowToBody { get; init; } = 2m;

    /// <summary>Star-style patterns: middle candle body &lt;= this * first candle body.</summary>
    public decimal StarBodyToFirstBody { get; init; } = 0.3m;

    /// <summary>Three soldiers/crows: trailing-side shadow &lt;= this * body.</summary>
    public decimal SoldierMaxShadowToBody { get; init; } = 0.5m;

    /// <summary>Candles examined before a pattern to classify trend context.</summary>
    public int TrendLookback { get; init; } = 5;
}
