namespace ZTrade.Core.Market;

/// <summary>
/// Direction reported by the exchange for an individual trade. Unknown means the feed did not
/// provide enough information; it is deliberately not inferred from candle color.
/// </summary>
public enum TradeSide
{
    Unknown,
    Buy,
    Sell,
}

/// <summary>An exchange trade print. Timestamps are Unix milliseconds; price and size are exact decimals.</summary>
public sealed record MarketTrade(
    long TimestampMilliseconds,
    decimal Price,
    decimal Quantity,
    TradeSide Side,
    string? TradeId = null);

/// <summary>Best bid/ask when the upstream feed supplies them. Sizes may be absent.</summary>
public sealed record MarketQuote(
    long TimestampMilliseconds,
    decimal? Bid,
    decimal? BidSize,
    decimal? Ask,
    decimal? AskSize,
    decimal? Change24hPercent = null,
    decimal? Volume24h = null);

/// <summary>The result of applying one real trade to a streaming candle builder.</summary>
public readonly record struct CandleUpdate(
    bool Accepted,
    bool StartedNewBar,
    Candle? ClosedCandle,
    Candle? ActiveCandle,
    bool GapDetected = false);

/// <summary>
/// Incremental OHLCV aggregator for real exchange trades. It uses a fixed-capacity history buffer,
/// ignores late prints that would rewrite a closed candle, and never invents candles across feed gaps.
/// The class is intentionally single-writer; readers should consume <see cref="Snapshot"/> copies.
/// </summary>
public sealed class RealtimeCandleAggregator
{
    private readonly List<Candle> _closed = new();
    private readonly int _capacity;
    private readonly long _intervalSeconds;
    private Candle? _active;
    private long? _lastTradeTimestampMilliseconds;
    private decimal _activeBuyVolume;
    private decimal _activeSellVolume;

    public RealtimeCandleAggregator(
        CandleInterval interval,
        IReadOnlyList<Candle>? seed = null,
        int capacity = 2000)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 2);
        if (!Enum.IsDefined(interval)) throw new ArgumentOutOfRangeException(nameof(interval));
        _capacity = capacity;
        Interval = interval;
        _intervalSeconds = interval.Seconds();
        if (seed is null) return;

        CandleSeries.EnsureAscending(seed);
        var first = Math.Max(0, seed.Count - capacity);
        for (var i = first; i < seed.Count; i++) _closed.Add(seed[i]);
    }

    public CandleInterval Interval { get; }

    public Candle? ActiveCandle => _active;

    public Candle? LatestCandle => _active ?? (_closed.Count > 0 ? _closed[^1] : null);

    public decimal ActiveBuyVolume => _activeBuyVolume;

    public decimal ActiveSellVolume => _activeSellVolume;

    public CandleUpdate Update(MarketTrade trade)
    {
        ArgumentNullException.ThrowIfNull(trade);
        if (trade.TimestampMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(trade), "Trade time must be >= 0.");
        if (trade.Price <= 0m) throw new ArgumentOutOfRangeException(nameof(trade), "Trade price must be > 0.");
        if (trade.Quantity < 0m) throw new ArgumentOutOfRangeException(nameof(trade), "Trade quantity must be >= 0.");
        if (_lastTradeTimestampMilliseconds is { } lastTradeTime && trade.TimestampMilliseconds < lastTradeTime)
        {
            return new CandleUpdate(false, false, null, _active);
        }

        var timestampSeconds = trade.TimestampMilliseconds / 1000;
        var barTimestamp = timestampSeconds - timestampSeconds % _intervalSeconds;
        var lastTimestamp = _active?.Timestamp ?? (_closed.Count > 0 ? _closed[^1].Timestamp : -1);

        // A REST backfill contains closed bars only. If a delayed print belongs to that history,
        // drop it rather than silently mutating a supposedly-finalized candle.
        if (barTimestamp < lastTimestamp || (_active is null && barTimestamp == lastTimestamp))
        {
            return new CandleUpdate(false, false, null, _active);
        }

        if (_active is null || barTimestamp > _active.Value.Timestamp)
        {
            Candle? closed = null;
            var gapDetected = lastTimestamp >= 0 && barTimestamp - lastTimestamp > _intervalSeconds;
            if (_active is { } previous)
            {
                // A skipped timeframe is visible as a data gap, not presented to strategies as a complete candle.
                if (barTimestamp - previous.Timestamp == _intervalSeconds) closed = previous;
                else gapDetected = true;
                _closed.Add(previous);
            }

            TrimClosed();
            _active = new Candle(barTimestamp, trade.Price, trade.Price, trade.Price, trade.Price, trade.Quantity);
            _activeBuyVolume = trade.Side == TradeSide.Buy ? trade.Quantity : 0m;
            _activeSellVolume = trade.Side == TradeSide.Sell ? trade.Quantity : 0m;
            _lastTradeTimestampMilliseconds = trade.TimestampMilliseconds;
            return new CandleUpdate(true, true, closed, _active, gapDetected);
        }

        var current = _active.Value;
        _active = new Candle(
            current.Timestamp,
            current.Open,
            Math.Max(current.High, trade.Price),
            Math.Min(current.Low, trade.Price),
            trade.Price,
            current.Volume + trade.Quantity);
        if (trade.Side == TradeSide.Buy) _activeBuyVolume += trade.Quantity;
        else if (trade.Side == TradeSide.Sell) _activeSellVolume += trade.Quantity;
        _lastTradeTimestampMilliseconds = trade.TimestampMilliseconds;
        return new CandleUpdate(true, false, null, _active);
    }

    /// <summary>Copies the most recent closed bars and the current forming bar, if any.</summary>
    public IReadOnlyList<Candle> Snapshot()
    {
        var result = new List<Candle>(_closed.Count + (_active is null ? 0 : 1));
        result.AddRange(_closed);
        if (_active is { } active) result.Add(active);
        return result;
    }

    /// <summary>Copies at most <paramref name="count"/> latest candles, including the forming bar.</summary>
    public IReadOnlyList<Candle> Tail(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        var resultCount = _closed.Count + (_active is null ? 0 : 1);
        var skip = Math.Max(0, resultCount - count);
        var result = new List<Candle>(Math.Min(resultCount, count));
        if (skip < _closed.Count)
        {
            for (var i = skip; i < _closed.Count; i++) result.Add(_closed[i]);
        }

        if (_active is { } active && result.Count < count) result.Add(active);
        return result;
    }

    private void TrimClosed()
    {
        // Reserve one slot for the active candle so the visible series never exceeds capacity.
        var maximumClosed = Math.Max(0, _capacity - 1);
        var excess = _closed.Count - maximumClosed;
        if (excess > 0) _closed.RemoveRange(0, excess);
    }
}
