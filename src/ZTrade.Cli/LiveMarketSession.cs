using System.Diagnostics;
using System.Net.WebSockets;
using System.Text.Json;
using ZTrade.Core.Market;
using ZTrade.Exchanges.Live;
using ZTrade.Trading;

namespace ZTrade.Cli;

/// <summary>Reconnect supervisor and bounded-rate bridge from exchange events into dashboard state.</summary>
public sealed class LiveMarketSession
{
    private static readonly TimeSpan PublishInterval = TimeSpan.FromMilliseconds(50);
    private readonly ILiveMarketFeed _feed;
    private readonly LiveMarketState _state;
    private readonly LivePaperEngine? _paper;

    public LiveMarketSession(ILiveMarketFeed feed, LiveMarketState state, LivePaperEngine? paper = null)
    {
        _feed = feed ?? throw new ArgumentNullException(nameof(feed));
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _paper = paper;
        if (_paper is not null) _state.SetPaperSnapshot(_paper.Snapshot());
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        var connectedAt = 0L;
        while (!cancellationToken.IsCancellationRequested)
        {
            _state.SetConnectionState(attempt == 0 ? "connecting" : "reconnecting", null);
            _state.Publish();
            try
            {
                await using var enumerator = _feed.StreamAsync(_state.Symbol, cancellationToken)
                    .GetAsyncEnumerator(cancellationToken);
                using var timer = new PeriodicTimer(PublishInterval);
                var moveTask = enumerator.MoveNextAsync().AsTask();
                var timerTask = timer.WaitForNextTickAsync(cancellationToken).AsTask();
                var dirty = false;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var completed = await Task.WhenAny(moveTask, timerTask).ConfigureAwait(false);
                    if (completed == moveTask)
                    {
                        if (!await moveTask.ConfigureAwait(false))
                        {
                            throw new WebSocketException("The exchange closed its market-data stream.");
                        }

                        Apply(enumerator.Current, ref connectedAt, ref attempt);
                        dirty = true;
                        moveTask = enumerator.MoveNextAsync().AsTask();
                    }

                    if (completed == timerTask)
                    {
                        if (!await timerTask.ConfigureAwait(false)) break;
                        if (dirty)
                        {
                            _state.Publish();
                            dirty = false;
                        }

                        timerTask = timer.WaitForNextTickAsync(cancellationToken).AsTask();
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex) when (ex is WebSocketException or IOException or HttpRequestException
                                               or JsonException or FormatException or TimeoutException)
            {
                if (connectedAt != 0 && Stopwatch.GetElapsedTime(connectedAt) >= TimeSpan.FromSeconds(15)) attempt = 0;
                attempt = Math.Min(attempt + 1, 8);
                _state.SetConnectionState("reconnecting", ex.Message);
                _state.Publish();
                var baseMilliseconds = Math.Min(500d * Math.Pow(2d, attempt - 1), 15_000d);
                var delay = TimeSpan.FromMilliseconds(baseMilliseconds + Random.Shared.NextDouble() * baseMilliseconds * 0.25d);
                try { await Task.Delay(delay, cancellationToken).ConfigureAwait(false); }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { break; }
            }
        }

        _state.SetConnectionState("stopped", "Live feed stopped.");
        _state.Publish();
    }

    private void Apply(LiveFeedEvent update, ref long connectedAt, ref int attempt)
    {
        switch (update)
        {
            case LiveFeedConnection { Connected: true } connection:
                // On reconnect, an existing active candle and the first post-reconnect candle can both
                // be partial when the socket resumes on the other side of a bar boundary.
                var reconnectHasActiveBar = connectedAt != 0 && _state.HasActiveCandle;
                _paper?.SuppressNextClosedSignals(reconnectHasActiveBar ? 2 : 1);
                if (connectedAt != 0 && Stopwatch.GetElapsedTime(connectedAt) >= TimeSpan.FromSeconds(15)) attempt = 0;
                connectedAt = Stopwatch.GetTimestamp();
                _state.SetConnectionState("live", connection.Detail);
                break;

            case LiveFeedTrade tradeUpdate:
                var trade = tradeUpdate.Trade;
                var candleUpdate = _state.ApplyTrade(trade);
                if (!candleUpdate.Accepted) break;
                if (_paper is not null)
                {
                    if (candleUpdate.StartedNewBar)
                    {
                        if (candleUpdate.GapDetected) _paper.SuppressNextClosedSignal();
                        else _ = _paper.OnBarOpened(trade.TimestampMilliseconds / 1000, trade.Price);
                    }

                    if (candleUpdate.ClosedCandle is { } closed) _ = _paper.OnBarClosed(in closed);
                    _paper.Mark(trade.Price);
                    _state.SetPaperSnapshot(_paper.Snapshot());
                }

                break;

            case LiveFeedQuote quoteUpdate:
                _state.ApplyQuote(quoteUpdate.Quote);
                break;
        }
    }
}
