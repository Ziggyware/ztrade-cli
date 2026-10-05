using System.Threading.Channels;
using ZTrade.Core.Market;
using ZTrade.Trading;

namespace ZTrade.Cli;

public sealed record LiveCandleView(
    long Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);

public sealed record LiveTradeView(
    long TimestampMilliseconds,
    decimal Price,
    decimal Quantity,
    string Side,
    string? TradeId);

/// <summary>Snapshot for first load; stream updates omit the full candle history.</summary>
public sealed record LiveMarketSnapshot(
    string Exchange,
    string Symbol,
    string Interval,
    string ConnectionState,
    string? ConnectionDetail,
    long Sequence,
    long ServerTimeMilliseconds,
    long? LastTradeTimeMilliseconds,
    decimal? LastPrice,
    decimal? Bid,
    decimal? BidSize,
    decimal? Ask,
    decimal? AskSize,
    decimal? SpreadBasisPoints,
    decimal? BookImbalance,
    decimal? Change24hPercent,
    decimal? Volume24h,
    decimal? SessionVwap,
    decimal BuyVolume,
    decimal SellVolume,
    decimal? TradeRatePerSecond,
    double? FeedLatencyMilliseconds,
    long TotalTrades,
    long DataGapCount,
    LiveCandleView? CurrentCandle,
    IReadOnlyList<LiveCandleView>? Candles,
    IReadOnlyList<LiveCandleView> CandleTail,
    IReadOnlyList<LiveTradeView> RecentTrades,
    LivePaperSnapshot? Paper);

/// <summary>
/// Thread-safe live dashboard state. Trade aggregation itself is incremental; the browser receives a full
/// history only on connect and bounded, latest-value deltas thereafter.
/// </summary>
public sealed class LiveMarketState
{
    private const int RecentTradeCapacity = 32;
    private const int RecentTimestampCapacity = 8192;
    private const int RecentTradeIdCapacity = 2048;
    private static readonly TimeSpan RateWindow = TimeSpan.FromSeconds(10);

    private readonly object _gate = new();
    private readonly RealtimeCandleAggregator _aggregator;
    private readonly TimeProvider _time;
    private readonly Queue<LiveTradeView> _recentTrades = new();
    private readonly Queue<long> _arrivals = new();
    private readonly Queue<string> _tradeIdOrder = new();
    private readonly HashSet<string> _tradeIds = new(StringComparer.Ordinal);
    private readonly List<Channel<LiveMarketSnapshot>> _subscribers = new();
    private MarketQuote? _quote;
    private LivePaperSnapshot? _paper;
    private string _connectionState = "connecting";
    private string? _connectionDetail;
    private long _sequence;
    private long _totalTrades;
    private long _dataGapCount;
    private long? _lastTradeTimeMilliseconds;
    private decimal? _lastPrice;
    private decimal _sessionVolume;
    private decimal _sessionNotional;

    public LiveMarketState(
        string exchange,
        string symbol,
        CandleInterval interval,
        IReadOnlyList<Candle>? seed = null,
        int maxCandles = 2000,
        TimeProvider? timeProvider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exchange);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxCandles, 2);
        Exchange = exchange;
        Symbol = symbol;
        Interval = interval;
        _time = timeProvider ?? TimeProvider.System;
        _aggregator = new RealtimeCandleAggregator(interval, seed, maxCandles);
    }

    public string Exchange { get; }
    public string Symbol { get; }
    public CandleInterval Interval { get; }

    public bool HasActiveCandle
    {
        get { lock (_gate) return _aggregator.ActiveCandle is not null; }
    }

    public CandleUpdate ApplyTrade(MarketTrade trade)
    {
        ArgumentNullException.ThrowIfNull(trade);
        lock (_gate)
        {
            if (trade.TradeId is { Length: > 0 } tradeId && _tradeIds.Contains(tradeId))
            {
                return new CandleUpdate(false, false, null, _aggregator.ActiveCandle);
            }

            var update = _aggregator.Update(trade);
            if (!update.Accepted) return update;

            if (trade.TradeId is { Length: > 0 } acceptedId)
            {
                _tradeIds.Add(acceptedId);
                _tradeIdOrder.Enqueue(acceptedId);
                while (_tradeIdOrder.Count > RecentTradeIdCapacity)
                {
                    _tradeIds.Remove(_tradeIdOrder.Dequeue());
                }
            }

            var arrivalMilliseconds = _time.GetUtcNow().ToUnixTimeMilliseconds();
            _arrivals.Enqueue(arrivalMilliseconds);
            while (_arrivals.Count > RecentTimestampCapacity) _arrivals.Dequeue();
            TrimArrivalWindow(arrivalMilliseconds);

            if (update.GapDetected) _dataGapCount++;
            _lastTradeTimeMilliseconds = trade.TimestampMilliseconds;
            _lastPrice = trade.Price;
            _totalTrades++;
            _sessionVolume += trade.Quantity;
            _sessionNotional += trade.Price * trade.Quantity;
            _recentTrades.Enqueue(new LiveTradeView(
                trade.TimestampMilliseconds,
                trade.Price,
                trade.Quantity,
                trade.Side.ToString().ToLowerInvariant(),
                trade.TradeId));
            while (_recentTrades.Count > RecentTradeCapacity) _recentTrades.Dequeue();
            return update;
        }
    }

    public void ApplyQuote(MarketQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        lock (_gate)
        {
            if (_quote is { } previous && quote.TimestampMilliseconds < previous.TimestampMilliseconds) return;
            _quote = quote;
        }
    }

    public void SetConnectionState(string state, string? detail = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        lock (_gate)
        {
            _connectionState = state;
            _connectionDetail = detail is { Length: > 240 } ? detail[..240] : detail;
            if (!string.Equals(state, "live", StringComparison.OrdinalIgnoreCase)) _quote = null;
        }
    }

    public void SetPaperSnapshot(LivePaperSnapshot? paper)
    {
        lock (_gate) _paper = paper;
    }

    /// <summary>Creates a latest-value stream. A slow browser is coalesced to one pending update, never queued unboundedly.</summary>
    public LiveMarketSubscription Subscribe()
    {
        lock (_gate)
        {
            var channel = Channel.CreateBounded<LiveMarketSnapshot>(new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true,
            });
            _subscribers.Add(channel);
            return new LiveMarketSubscription(CreateSnapshot(includeCandles: true), channel, RemoveSubscriber);
        }
    }

    public LiveMarketSnapshot Snapshot()
    {
        lock (_gate) return CreateSnapshot(includeCandles: true);
    }

    /// <summary>Broadcasts a compact delta to browser clients. Callers should coalesce fast ticks (about 20 Hz is ample).</summary>
    public void Publish()
    {
        lock (_gate)
        {
            _sequence++;
            var update = CreateSnapshot(includeCandles: false);
            foreach (var subscriber in _subscribers) subscriber.Writer.TryWrite(update);
        }
    }

    private LiveMarketSnapshot CreateSnapshot(bool includeCandles)
    {
        var now = _time.GetUtcNow();
        var nowMilliseconds = now.ToUnixTimeMilliseconds();
        var current = _aggregator.ActiveCandle;
        var quote = _quote;
        decimal? spreadBps = null;
        decimal? imbalance = null;
        if (quote?.Bid is { } bid && quote.Ask is { } ask && bid > 0m && ask >= bid)
        {
            var mid = (bid + ask) / 2m;
            if (mid > 0m) spreadBps = (ask - bid) / mid * 10_000m;
        }

        if (quote?.BidSize is { } bidSize && quote.AskSize is { } askSize && bidSize + askSize > 0m)
        {
            imbalance = (bidSize - askSize) / (bidSize + askSize);
        }

        var rate = CalculateTradeRate(nowMilliseconds);
        var latency = _lastTradeTimeMilliseconds is { } exchangeTime
            ? (double)Math.Max(0, nowMilliseconds - exchangeTime)
            : (double?)null;
        return new LiveMarketSnapshot(
            Exchange,
            Symbol,
            Interval.ToShortString(),
            _connectionState,
            _connectionDetail,
            _sequence,
            nowMilliseconds,
            _lastTradeTimeMilliseconds,
            _lastPrice,
            quote?.Bid,
            quote?.BidSize,
            quote?.Ask,
            quote?.AskSize,
            spreadBps,
            imbalance,
            quote?.Change24hPercent,
            quote?.Volume24h,
            _sessionVolume > 0m ? _sessionNotional / _sessionVolume : null,
            _aggregator.ActiveBuyVolume,
            _aggregator.ActiveSellVolume,
            rate,
            latency,
            _totalTrades,
            _dataGapCount,
            current is { } active ? ToView(active) : null,
            includeCandles ? _aggregator.Snapshot().Select(ToView).ToArray() : null,
            _aggregator.Tail(3).Select(ToView).ToArray(),
            _recentTrades.ToArray(),
            _paper);
    }

    private static LiveCandleView ToView(Candle candle) =>
        new(candle.Timestamp, candle.Open, candle.High, candle.Low, candle.Close, candle.Volume);

    private decimal? CalculateTradeRate(long nowMilliseconds)
    {
        TrimArrivalWindow(nowMilliseconds);
        if (_arrivals.Count == 0) return 0m;
        var durationMilliseconds = Math.Max(1, nowMilliseconds - _arrivals.Peek());
        var sampleMilliseconds = Math.Min((long)RateWindow.TotalMilliseconds, durationMilliseconds);
        return _arrivals.Count * 1000m / sampleMilliseconds;
    }

    private void TrimArrivalWindow(long nowMilliseconds)
    {
        var earliest = nowMilliseconds - (long)RateWindow.TotalMilliseconds;
        while (_arrivals.Count > 0 && _arrivals.Peek() < earliest) _arrivals.Dequeue();
    }

    private void RemoveSubscriber(Channel<LiveMarketSnapshot> channel)
    {
        lock (_gate)
        {
            if (_subscribers.Remove(channel)) channel.Writer.TryComplete();
        }
    }
}

public sealed class LiveMarketSubscription : IDisposable
{
    private readonly Action<Channel<LiveMarketSnapshot>> _unsubscribe;
    private int _disposed;

    internal LiveMarketSubscription(
        LiveMarketSnapshot initialSnapshot,
        Channel<LiveMarketSnapshot> channel,
        Action<Channel<LiveMarketSnapshot>> unsubscribe)
    {
        InitialSnapshot = initialSnapshot;
        Reader = channel.Reader;
        _unsubscribe = unsubscribe;
        Channel = channel;
    }

    private Channel<LiveMarketSnapshot> Channel { get; }
    public LiveMarketSnapshot InitialSnapshot { get; }
    public ChannelReader<LiveMarketSnapshot> Reader { get; }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _unsubscribe(Channel);
    }
}
