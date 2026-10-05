using ZTrade.Core.Market;

namespace ZTrade.Trading;

/// <summary>A compact immutable view of a running, explicitly simulated account.</summary>
public sealed record LivePaperSnapshot(
    string StrategyName,
    decimal InitialCash,
    decimal Cash,
    decimal Position,
    decimal MarkPrice,
    decimal Equity,
    decimal ReturnFraction,
    string PendingSignal,
    int TradeCount,
    int WinCount,
    IReadOnlyList<Fill> RecentFills,
    IReadOnlyList<Trade> RecentTrades);

/// <summary>
/// Drives the existing fee-aware paper broker from live closed candles. It has no exchange credentials,
/// network methods, or order-routing interface. Signals use the same no-lookahead next-bar-open rule as backtests.
/// </summary>
public sealed class LivePaperEngine
{
    private readonly IStrategy _strategy;
    private readonly PaperBroker _broker;
    private Signal _pending = Signal.Hold;
    private decimal _markPrice;
    private int _closedSignalsToSuppress;

    public LivePaperEngine(
        IStrategy strategy,
        decimal initialCash,
        decimal feeRate,
        decimal slippageRate,
        IReadOnlyList<Candle>? warmup = null)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _broker = new PaperBroker(initialCash, feeRate, slippageRate);
        _strategy.Reset();
        if (warmup is null) return;

        CandleSeries.EnsureAscending(warmup);
        foreach (var candle in warmup)
        {
            // Warm indicator state without replaying historical signals into a newly-created account.
            _ = _strategy.OnCandle(candle);
        }
    }

    /// <summary>Called on the first trade of a new candle, before that candle is evaluated.</summary>
    public Fill? OnBarOpened(long timestamp, decimal price)
    {
        _markPrice = price;
        var pending = _pending;
        _pending = Signal.Hold;
        return pending switch
        {
            Signal.Buy => _broker.Buy(timestamp, price),
            Signal.Sell => _broker.Sell(timestamp, price),
            _ => null,
        };
    }

    /// <summary>Calculates a next-bar signal from a finalized candle. No current-bar look-ahead.</summary>
    public Signal OnBarClosed(in Candle candle)
    {
        _markPrice = candle.Close;
        if (_closedSignalsToSuppress > 0)
        {
            _closedSignalsToSuppress--;
            _pending = Signal.Hold;
            return _pending;
        }

        _pending = _strategy.OnCandle(candle);
        return _pending;
    }

    /// <summary>Skips possibly partial candle signals after startup, reconnects, or feed gaps.</summary>
    public void SuppressNextClosedSignal() => SuppressNextClosedSignals(1);

    /// <summary>Skips at least <paramref name="count"/> subsequent closed-bar signals.</summary>
    public void SuppressNextClosedSignals(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        _pending = Signal.Hold;
        _closedSignalsToSuppress = Math.Max(_closedSignalsToSuppress, count);
    }

    /// <summary>Marks the simulated position to the latest real trade without generating an order.</summary>
    public void Mark(decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0m);
        _markPrice = price;
    }

    public LivePaperSnapshot Snapshot()
    {
        var equity = _broker.Equity(_markPrice > 0m ? _markPrice : 0m);
        return new LivePaperSnapshot(
            _strategy.Name,
            _broker.InitialCash,
            _broker.Cash,
            _broker.Position,
            _markPrice,
            equity,
            equity / _broker.InitialCash - 1m,
            _pending.ToString(),
            _broker.Trades.Count,
            _broker.Trades.Count(trade => trade.IsWin),
            _broker.Fills.TakeLast(12).ToArray(),
            _broker.Trades.TakeLast(12).ToArray());
    }
}
