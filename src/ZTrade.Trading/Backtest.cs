using ZTrade.Core.Market;

namespace ZTrade.Trading;

public sealed record BacktestResult(
    string StrategyName,
    int CandleCount,
    decimal InitialEquity,
    decimal FinalEquity,
    decimal ReturnFraction,
    decimal BuyAndHoldReturnFraction,
    decimal MaxDrawdownFraction,
    int TradeCount,
    int WinCount,
    bool EndedInPosition,
    IReadOnlyList<Trade> Trades,
    IReadOnlyList<Fill> Fills)
{
    public decimal? WinRate => TradeCount == 0 ? null : (decimal)WinCount / TradeCount;
}

public static class BacktestEngine
{
    /// <summary>
    /// Replays candles through a strategy. A signal produced from candle i's CLOSE is executed at candle i+1's OPEN
    /// (no look-ahead). An open position at the end is marked to market, not force-closed.
    /// Equity is marked at each close; drawdown is measured on that close-to-close curve.
    /// </summary>
    public static BacktestResult Run(IReadOnlyList<Candle> candles, IStrategy strategy, PaperBroker broker)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        ArgumentNullException.ThrowIfNull(broker);
        CandleSeries.EnsureAscending(candles);
        if (candles.Count < 2) throw new ArgumentException("Need at least 2 candles.", nameof(candles));

        strategy.Reset();
        var pending = Signal.Hold;
        var peak = broker.InitialCash;
        var maxDrawdown = 0m;

        for (var i = 0; i < candles.Count; i++)
        {
            var candle = candles[i];

            if (pending == Signal.Buy) broker.Buy(candle.Timestamp, candle.Open);
            else if (pending == Signal.Sell) broker.Sell(candle.Timestamp, candle.Open);

            var equity = broker.Equity(candle.Close);
            peak = Math.Max(peak, equity);
            maxDrawdown = Math.Max(maxDrawdown, (peak - equity) / peak);

            pending = strategy.OnCandle(candle);
        }

        var finalEquity = broker.Equity(candles[^1].Close);
        var wins = broker.Trades.Count(t => t.IsWin);
        return new BacktestResult(
            strategy.Name,
            candles.Count,
            broker.InitialCash,
            finalEquity,
            finalEquity / broker.InitialCash - 1m,
            candles[^1].Close / candles[0].Open - 1m,
            maxDrawdown,
            broker.Trades.Count,
            wins,
            broker.IsInPosition,
            broker.Trades.ToArray(),
            broker.Fills.ToArray());
    }
}
