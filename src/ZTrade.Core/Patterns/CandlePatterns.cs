using ZTrade.Core.Market;

namespace ZTrade.Core.Patterns;

/// <summary>
/// Pure, allocation-free candlestick shape predicates. Context (trend) is applied by <see cref="PatternScanner"/>.
/// Where the legacy ZTrade formula was internally inconsistent with the named pattern (harami, piercing),
/// these use the standard textbook definition; see README "Behavioral differences".
/// </summary>
public static class CandlePatterns
{
    // ---- single candle -------------------------------------------------

    public static bool IsDoji(in Candle c, PatternOptions o) =>
        c.Range > 0m && c.Body <= o.DojiBodyToShadows * (c.UpperShadow + c.LowerShadow);

    public static bool IsDragonflyDoji(in Candle c, PatternOptions o) =>
        IsDoji(c, o)
        && c.UpperShadow <= o.DojiShortShadowFraction * c.Range
        && c.LowerShadow >= o.DojiLongShadowFraction * c.Range;

    public static bool IsGravestoneDoji(in Candle c, PatternOptions o) =>
        IsDoji(c, o)
        && c.LowerShadow <= o.DojiShortShadowFraction * c.Range
        && c.UpperShadow >= o.DojiLongShadowFraction * c.Range;

    public static bool IsLongLeggedDoji(in Candle c, PatternOptions o) =>
        IsDoji(c, o)
        && c.UpperShadow >= o.LongLeggedMinShadowFraction * c.Range
        && c.LowerShadow >= o.LongLeggedMinShadowFraction * c.Range;

    /// <summary>Long lower shadow, small upper shadow. Hammer after a downtrend; hanging man after an uptrend.</summary>
    public static bool HasHammerShape(in Candle c, PatternOptions o) =>
        c.Body > 0m && c.LowerShadow >= o.HammerShadowToBody * c.Body && c.UpperShadow < c.Body;

    /// <summary>Long upper shadow, small lower shadow. Inverted hammer after a downtrend; shooting star after an uptrend.</summary>
    public static bool HasInvertedHammerShape(in Candle c, PatternOptions o) =>
        c.Body > 0m && c.UpperShadow >= o.HammerShadowToBody * c.Body && c.LowerShadow < c.Body;

    // ---- two candles (previous, current) --------------------------------

    public static bool IsBullishEngulfing(in Candle prev, in Candle cur) =>
        prev.IsBearish && cur.IsBullish && cur.Close > prev.Open && cur.Open < prev.Close;

    public static bool IsBearishEngulfing(in Candle prev, in Candle cur) =>
        prev.IsBullish && cur.IsBearish && cur.Close < prev.Open && cur.Open > prev.Close;

    public static bool IsBullishHarami(in Candle prev, in Candle cur, PatternOptions o) =>
        prev.IsBearish && cur.IsBullish && !IsDoji(cur, o) && cur.Open > prev.Close && cur.Close < prev.Open;

    public static bool IsBearishHarami(in Candle prev, in Candle cur, PatternOptions o) =>
        prev.IsBullish && cur.IsBearish && !IsDoji(cur, o) && cur.Open < prev.Close && cur.Close > prev.Open;

    public static bool IsBullishHaramiCross(in Candle prev, in Candle cur, PatternOptions o) =>
        prev.IsBearish && IsDoji(cur, o) && cur.BodyHigh < prev.Open && cur.BodyLow > prev.Close;

    public static bool IsBearishHaramiCross(in Candle prev, in Candle cur, PatternOptions o) =>
        prev.IsBullish && IsDoji(cur, o) && cur.BodyHigh < prev.Close && cur.BodyLow > prev.Open;

    /// <summary>Opens below prior close, closes above the prior body's midpoint but below its open.</summary>
    public static bool IsBullishPiercing(in Candle prev, in Candle cur) =>
        prev.IsBearish && cur.IsBullish
        && cur.Open < prev.Close && cur.Close > prev.BodyMidpoint && cur.Close < prev.Open;

    /// <summary>Opens above prior close, closes below the prior body's midpoint but above its open.</summary>
    public static bool IsBearishDarkCloudCover(in Candle prev, in Candle cur) =>
        prev.IsBullish && cur.IsBearish
        && cur.Open > prev.Close && cur.Close < prev.BodyMidpoint && cur.Close > prev.Open;

    // ---- three candles --------------------------------------------------

    public static bool IsBullishMorningStar(in Candle a, in Candle b, in Candle c, PatternOptions o) =>
        a.IsBearish
        && b.Body <= o.StarBodyToFirstBody * a.Body
        && b.BodyHigh <= a.Close
        && c.IsBullish
        && c.Close > a.BodyMidpoint;

    public static bool IsBearishEveningStar(in Candle a, in Candle b, in Candle c, PatternOptions o) =>
        a.IsBullish
        && b.Body <= o.StarBodyToFirstBody * a.Body
        && b.BodyLow >= a.Close
        && c.IsBearish
        && c.Close < a.BodyMidpoint;

    public static bool IsBullishThreeWhiteSoldiers(in Candle a, in Candle b, in Candle c, PatternOptions o) =>
        a.IsBullish && b.IsBullish && c.IsBullish
        && b.Close > a.Close && c.Close > b.Close
        && b.Open > a.Open && b.Open < a.Close
        && c.Open > b.Open && c.Open < b.Close
        && a.UpperShadow <= o.SoldierMaxShadowToBody * a.Body
        && b.UpperShadow <= o.SoldierMaxShadowToBody * b.Body
        && c.UpperShadow <= o.SoldierMaxShadowToBody * c.Body;

    public static bool IsBearishThreeBlackCrows(in Candle a, in Candle b, in Candle c, PatternOptions o) =>
        a.IsBearish && b.IsBearish && c.IsBearish
        && b.Close < a.Close && c.Close < b.Close
        && b.Open < a.Open && b.Open > a.Close
        && c.Open < b.Open && c.Open > b.Close
        && a.LowerShadow <= o.SoldierMaxShadowToBody * a.Body
        && b.LowerShadow <= o.SoldierMaxShadowToBody * b.Body
        && c.LowerShadow <= o.SoldierMaxShadowToBody * c.Body;
}
