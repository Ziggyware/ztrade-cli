using ZTrade.Core.Market;

namespace ZTrade.Core.Patterns;

public static class PatternScanner
{
    /// <summary>
    /// Scans an ascending series. A pattern is reported at the index of its LAST candle, using only candles
    /// at or before that index (no look-ahead), so results are safe to use in a backtest.
    /// </summary>
    public static IReadOnlyList<PatternMatch> Scan(IReadOnlyList<Candle> candles, PatternOptions? options = null)
    {
        options ??= new PatternOptions();
        CandleSeries.EnsureAscending(candles);

        var matches = new List<PatternMatch>();
        for (var i = 0; i < candles.Count; i++)
        {
            var cur = candles[i];
            var index = i;

            void Add(PatternType type, int span) => matches.Add(new PatternMatch(type, index - span + 1, index, cur.Timestamp));

            // Trend measured over the lookback window ending just before the pattern's first candle.
            TrendDirection TrendBefore(int span) =>
                index - span >= options.TrendLookback - 1 || index - span + 1 >= options.TrendLookback
                    ? Trend.Direction(candles, index - span + 1 - options.TrendLookback, options.TrendLookback)
                    : TrendDirection.None;

            // --- 1 candle
            if (CandlePatterns.IsDoji(cur, options))
            {
                if (CandlePatterns.IsDragonflyDoji(cur, options)) Add(PatternType.DragonflyDoji, 1);
                else if (CandlePatterns.IsGravestoneDoji(cur, options)) Add(PatternType.GravestoneDoji, 1);
                else if (CandlePatterns.IsLongLeggedDoji(cur, options)) Add(PatternType.LongLeggedDoji, 1);
                else Add(PatternType.StandardDoji, 1);
            }

            var trend1 = TrendBefore(1);
            if (CandlePatterns.HasHammerShape(cur, options))
            {
                if (trend1 == TrendDirection.Down) Add(PatternType.BullishHammer, 1);
                else if (trend1 == TrendDirection.Up) Add(PatternType.BearishHangingMan, 1);
            }

            if (CandlePatterns.HasInvertedHammerShape(cur, options))
            {
                if (trend1 == TrendDirection.Down) Add(PatternType.BullishInvertedHammer, 1);
                else if (trend1 == TrendDirection.Up) Add(PatternType.BearishShootingStar, 1);
            }

            // --- 2 candles
            if (i >= 1)
            {
                var prev = candles[i - 1];
                if (CandlePatterns.IsBullishEngulfing(prev, cur)) Add(PatternType.BullishEngulfing, 2);
                if (CandlePatterns.IsBearishEngulfing(prev, cur)) Add(PatternType.BearishEngulfing, 2);
                if (CandlePatterns.IsBullishHarami(prev, cur, options)) Add(PatternType.BullishHarami, 2);
                if (CandlePatterns.IsBearishHarami(prev, cur, options)) Add(PatternType.BearishHarami, 2);
                if (CandlePatterns.IsBullishHaramiCross(prev, cur, options)) Add(PatternType.BullishHaramiCross, 2);
                if (CandlePatterns.IsBearishHaramiCross(prev, cur, options)) Add(PatternType.BearishHaramiCross, 2);
                if (CandlePatterns.IsBullishPiercing(prev, cur)) Add(PatternType.BullishPiercing, 2);
                if (CandlePatterns.IsBearishDarkCloudCover(prev, cur)) Add(PatternType.BearishDarkCloudCover, 2);
            }

            // --- 3 candles
            if (i >= 2)
            {
                var a = candles[i - 2];
                var b = candles[i - 1];
                if (CandlePatterns.IsBullishMorningStar(a, b, cur, options)) Add(PatternType.BullishMorningStar, 3);
                if (CandlePatterns.IsBearishEveningStar(a, b, cur, options)) Add(PatternType.BearishEveningStar, 3);
                if (CandlePatterns.IsBullishThreeWhiteSoldiers(a, b, cur, options)) Add(PatternType.BullishThreeWhiteSoldiers, 3);
                if (CandlePatterns.IsBearishThreeBlackCrows(a, b, cur, options)) Add(PatternType.BearishThreeBlackCrows, 3);
            }
        }

        return matches;
    }
}
