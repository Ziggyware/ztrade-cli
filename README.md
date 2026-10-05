# ZTrade

Cross-platform .NET 8 market-analysis CLI with a **live browser market desk**, real-time public trade monitoring, a paper simulator, interactive historical charts, candlestick patterns, indicators, and backtesting.

**This application does not place live orders.** The live desk uses public exchange WebSockets and optional read-only candle history; paper mode simulates fills locally. No exchange credentials are read or sent.

## Start the live desk

```sh
# Gemini public prints + best bid/ask, with a local dashboard window
dotnet run --project src/ZTrade.Cli -- live --exchange gemini --symbol BTCUSD --interval 1m --count 300 --open

# Same real-time data, plus an isolated SMA-cross paper account
dotnet run --project src/ZTrade.Cli -- live --exchange gemini --symbol BTCUSD --paper sma \
   --fast 10 --slow 30 --cash 10000 --fee 0.002 --slippage 0.0005 --open

# Gate.io public spot trade/ticker stream
dotnet run --project src/ZTrade.Cli -- live --exchange gateio --symbol BTC_USDT --interval 1m --open
```

The server binds to `127.0.0.1:5175` by default. It prints the dashboard URL; `--open` launches the system browser. Ctrl+C stops both the dashboard and exchange stream. Use `--port` if that port is occupied. `--bind 0.0.0.0` exposes the **unauthenticated, read-only** dashboard to other machines on your network; leave the default loopback bind unless that is intentional.

Paper strategy values are `sma` and `rsi`; a bare `--paper` selects SMA. RSI accepts `--period`, `--oversold`, and `--overbought`. Paper fills are marked from real public trades and obey the same closed-bar / next-bar execution convention as backtests. Fees and slippage are assumptions, not exchange quotes.

## 30 live-desk features

### Real-time data and aggregation
1. Gemini public market-data WebSocket trade stream.
2. Gate.io v4 public spot trade stream.
3. Gemini price-level updates maintained into best bid/ask and displayed top-book sizes.
4. Gate.io spot ticker updates for best bid/ask and the exchange-reported 24-hour change and base volume.
5. REST candle backfill before the live stream starts.
6. Incremental OHLCV aggregation from individual public trade prints, using `decimal` values.
7. A visibly updating, still-forming candle alongside finalized history.
8. Delayed prints are prevented from rewriting an already-closed candle.
9. Missing intervals never create synthetic candles; detected gaps are counted and marked on the chart.
10. Bounded candle retention (`--max-bars`, default 2,000) to cap memory and browser work.
11. Recent exchange trade IDs are deduplicated across reconnects.
12. Buyer/seller aggressor side is shown only when supplied or derivable from exchange trade fields; unknown stays unknown.
13. Current-bar buy- and sell-initiated volume, rather than candle-color guesses.
14. Session VWAP computed from the individual trades received during this run.
15. Bounded time-and-sales tape with exchange timestamp, side, price, and size.
16. Best-bid/ask spread shown in basis points when both sides are available.
17. Top-book size imbalance shown only when the source provides both sizes.
18. Exchange-timestamp lag, observed print rate, and explicit live/reconnecting/stopped status.
19. Automatic reconnect with capped exponential backoff and jitter; Ctrl+C cancellation is propagated.

### Dashboard and responsiveness
20. A local browser dashboard launched with `--open`, not just a saved static chart file.
21. Server-Sent Events push live changes; `/api/snapshot` and `/healthz` support refresh and health checks.
22. Full retained candle history is sent on connect; compact latest-value updates follow. Each slow browser has a one-update bounded channel, so backlogs cannot grow without limit.
23. Trade aggregation stays incremental, dashboard updates are coalesced to at most about 20 per second, and canvas paint is coalesced with `requestAnimationFrame`.
24. High-DPI canvas rendering (device-pixel ratio capped at 2) with no browser chart CDN or third-party script.
25. Candlestick or close-line view, volume bars, range presets, fit, pointer pan, wheel zoom, crosshair OHLCV tooltip, follow-live toggle, PNG export, fullscreen, responsive narrow-screen layout, and keyboard shortcuts.

### Paper-only live simulation
26. Optional live-feed paper mode for SMA cross or RSI reversion.
27. Warm-up from the REST history without replaying historical signals into a new simulated account.
28. Signals use closed candles and are simulated at the next candle's first observed trade; partial startup/reconnect and post-gap bars are excluded from strategy state, with no same-candle look-ahead.
29. Configurable simulated starting cash, fee, and slippage, plus marked equity and return.
30. Simulated fill/trade summary and pending signal, visibly separated from the live market feed.

The dashboard does **not** claim hard real-time scheduling, exchange co-location, guaranteed low latency, or high-frequency execution. Actual throughput and timestamps depend on the network, exchange feed, operating system, and browser. The UI intentionally coalesces paint updates; it is for monitoring and paper simulation, not HFT.

## Other commands

```sh
dotnet build
dotnet run --project tests/ZTrade.Tests                         # dependency-free test runner
dotnet run --project src/ZTrade.Cli -- candles --symbol BTCUSD --interval 1hr --count 50 --out btc.csv
dotnet run --project src/ZTrade.Cli -- patterns --csv btc.csv
dotnet run --project src/ZTrade.Cli -- backtest --csv btc.csv --strategy sma --fast 10 --slow 30 --fee 0.002
dotnet run --project src/ZTrade.Cli -- chart --csv btc.csv --out btc.html --open --full
```

Exchange candle intervals vary by exchange. Gemini history supports `1m 5m 15m 30m 1hr 6hr 1day`; Gate.io history maps supported native spot widths (for example, CLI `1hr` to Gate.io `1h`). Live defaults to `1m`; when REST history is unavailable or that exchange does not support a width, the desk opens with live-only bars. Public feeds and candle endpoints do not use API keys.

The historical `chart` command creates a self-contained HTML file with the pattern ledger and 60 descriptive OHLCV studies; it remains useful offline, but it is not the live dashboard. See [interactive charting](src/ZTrade.Cli/CHARTING.md) for its study catalog and limitations.

## Legacy behavior and scope

The analytical rebuild intentionally differs from the original app on several pattern predicates:

| Pattern | Legacy behavior | Current behavior |
|---|---|---|
| Bullish harami | A branch matched a bearish shape; another allowed a partial engulf | Prior bearish candle, current bullish body contained within the prior body |
| Bullish piercing | Required a close above the prior open (an engulfing condition) | Opens below the prior close and closes above the prior-body midpoint, below the prior open |
| Trend context | Legacy `GetTrendAdvanceDecline` behavior could not be recovered | Least-squares slope over the recent closes |
| Morning/evening star, soldiers/crows | Range-to-close thresholds | Body-ratio thresholds in `PatternOptions` |
| Double top | Stub that always returned false | Not implemented |

Expect different pattern match counts than the legacy application on identical data. The original WinForms/Direct2D renderer, other historical exchange connectors, legacy chat/code-generation utilities, and real-money order placement remain out of scope. Gemini and Gate.io support here is public market data only.

## Project layout

```
src/ZTrade.Core       validated market domain, incremental indicators, candle patterns and scanner
src/ZTrade.Exchanges  Gemini/Gate.io public REST candles, WebSocket market-data feeds, CSV IO
src/ZTrade.Trading    SMA/RSI strategies, fee-aware PaperBroker, backtests, live paper signal runner
src/ZTrade.Cli        ztrade commands, local HTTP/SSE host, live graphical dashboard
tests/ZTrade.Tests    dependency-free test runner (non-zero exit on failure)
```

## Safety and data limits

- The exchange layer contains public market-data methods only; `PaperBroker` has no network client, API-key, or order-routing dependency. **No live order endpoints are implemented.**
- No credential variables are read. Do not add exchange secrets to source, chart HTML, URLs, browser storage, or logs. Any future execution feature should be a separately reviewed broker adapter with explicit opt-in, safeguards, and testnet-first design.
- Gemini top-of-book depth and Gate.io tickers have different data coverage. Unavailable values are shown as unavailable, not synthesized. Gate.io REST history uses base-currency candle volume to match live trade quantities and excludes the still-forming bar when its REST payload omits a close flag.
- `Session VWAP` and aggressor-flow totals cover prints received since this process started; they are not a full exchange session or reconstructed order book.
- Candle history may be shorter than requested due to upstream caps, endpoint coverage, or network errors. Exchange protocol fixtures are tested locally, but neither Gemini nor Gate.io REST/WebSocket behavior is exercised against production in the dependency-free test suite.
- The historical chart's candle-derived studies (delta, volume profile, VWAP proxies, sweeps, blocks, etc.) are estimates. Candle OHLCV does not reveal observed trades at price or true buyer/seller flow.
- The legacy source contained a commented-out Bard session cookie. Treat it as leaked and revoke it. Old signed-order code and keys are not part of this rebuild.

## Design notes

- Prices and volumes use `decimal`; candles are validated immutable OHLCV values. `default(Candle)` bypasses validation and is not market data.
- Backtests and live paper signals do not look ahead: a closed-bar signal is filled at the following bar's open/first observed trade. An unfinished final bar has no close signal.
- SMA uses an exact decimal running sum; indicators are incremental and reusable in streaming code.
- REST uses long-lived `HttpClient` instances, `PooledConnectionLifetime`, retry/backoff with jitter and cancellation. Live market data uses public `ClientWebSocket` streams and bounded browser fan-out.
- Nullable reference types, analyzers, deterministic builds, and warnings-as-errors are enabled.
