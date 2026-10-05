namespace ZTrade.Core.Patterns;

public enum PatternBias { Neutral, Bullish, Bearish }

public enum PatternType
{
    StandardDoji,
    LongLeggedDoji,
    DragonflyDoji,
    GravestoneDoji,
    BullishHammer,
    BearishHangingMan,
    BullishInvertedHammer,
    BearishShootingStar,
    BullishEngulfing,
    BearishEngulfing,
    BullishHarami,
    BearishHarami,
    BullishHaramiCross,
    BearishHaramiCross,
    BullishPiercing,
    BearishDarkCloudCover,
    BullishMorningStar,
    BearishEveningStar,
    BullishThreeWhiteSoldiers,
    BearishThreeBlackCrows,
}

public static class PatternTypeExtensions
{
    public static PatternBias Bias(this PatternType type) => type switch
    {
        PatternType.StandardDoji or PatternType.LongLeggedDoji or PatternType.DragonflyDoji
            or PatternType.GravestoneDoji => PatternBias.Neutral,
        PatternType.BullishHammer or PatternType.BullishInvertedHammer or PatternType.BullishEngulfing
            or PatternType.BullishHarami or PatternType.BullishHaramiCross or PatternType.BullishPiercing
            or PatternType.BullishMorningStar or PatternType.BullishThreeWhiteSoldiers => PatternBias.Bullish,
        _ => PatternBias.Bearish,
    };
}

/// <summary>A detected pattern. Indices are inclusive positions into the scanned series.</summary>
public readonly record struct PatternMatch(PatternType Type, int StartIndex, int EndIndex, long Timestamp)
{
    public PatternBias Bias => Type.Bias();
}
