# ZTrade (rebuild)

Cross-platform .NET 8 rebuild of the legacy ZTrade WinForms/Direct2D app's *analytical core*:
candles, candlestick-pattern detection, indicators, Gemini market data, and a paper-trading backtester.
**It cannot place orders.** The exchange layer is GET-only market data; `PaperBroker` is an in-memory simulation.

## Layout
```
src/ZTrade.Core       domain: Candle, intervals, indicators (SMA/EMA/RSI), pattern predicates + scanner (no dependencies)
src/ZTrade.Exchanges  IMarketDataClient, Gemini public candles client (retry/backoff), CSV import/export
src/ZTrade.Trading    IStrategy (SMA cross, RSI reversion), PaperBroker, BacktestEngine
src/ZTrade.Cli        `ztrade` command line + composition root
tests/ZTrade.Tests    unit tests (dependency-free runner; non-zero exit on failure)
```

## Use
```powershell
dotnet build
dotnet run --project tests/ZTrade.Tests                       # run tests
dotnet run --project src/ZTrade.Cli -- candles  --symbol BTCUSD --interval 1h --count 50 --out btc.csv
dotnet run --project src/ZTrade.Cli -- patterns --csv btc.csv
dotnet run --project src/ZTrade.Cli -- backtest --csv btc.csv --strategy sma --fast 10 --slow 30 --fee 0.002
# Generate and open the interactive chart in your default browser
dotnet run --project src/ZTrade.Cli -- chart --csv btc.csv --out btc.html --open --full
# Paper trading is an alias for the fee-aware backtest (never sends exchange orders)
dotnet run --project src/ZTrade.Cli -- paper --csv btc.csv --strategy rsi --cash 10000 --fee 0.001 --slippage 0.0005
```

Exchange intervals: `1m 5m 15m 30m 1h 6h 1d`. The Gemini candles endpoint is public; `GEMINI_EXCHANGE_API_KEY` is not read.

## Design decisions
- `decimal` prices/volumes (legacy used `double`): exact arithmetic on quoted values; cost is speed, irrelevant at this scale.
- `Candle` is a validated immutable struct (OHLC invariant enforced). `default(Candle)` bypasses this; never treat it as data.
- Backtest has no look-ahead: a signal from bar *i*'s close fills at bar *i+1*'s open; the final bar's signal is dropped; unclosed exchange candles are excluded by default.
- Pattern scanner reports each match at its last candle using only earlier candles (tested: scanning a prefix yields the same matches).
- Indicators are incremental (one implementation for batch and streaming); SMA keeps an exact decimal running sum.
- HTTP: one long-lived `HttpClient` (`PooledConnectionLifetime` set), retry on 429/5xx/network with exponential backoff + jitter, `Retry-After` honored, 4xx fails fast, cancellation end-to-end.
- Warnings are errors; nullable enabled; analyzers on (relaxed only for two noise rules under `tests/`).

## Behavioral differences from legacy (intentional)
| Pattern | Legacy | Here |
|---|---|---|
| Bullish harami | first branch encoded a *bearish* harami shape; second branch mere partial engulf | prior bearish, current bullish, current body inside prior body |
| Bullish piercing | required close > prior open (= engulfing) | opens below prior close, closes above prior-body midpoint, below prior open |
| Trend context | `GetTrendAdvanceDecline` (body not recovered from the dump) | sign of least-squares slope of the last 5 closes |
| Morning/evening star, soldiers/crows | `(high-low)/close < 0.2` style thresholds | body-ratio thresholds in `PatternOptions` |
| Double top | stub, always `false` | not ported |

Expect different match counts than the legacy app on identical data.

## Not ported
WinForms/Direct2D UI and the C++ renderer, XT/Bitmart/Bittrex/Coinbase/TXBit/CoinGecko clients, WebSockets, order placement,
the VinLib codegen/TFS/SQL utilities, the EdgeGPT/Bard/OpenAI chat code, head-and-shoulders, rounding tops/bottoms, kicking, abandoned baby.

## Security notes about the legacy source
- `Program.cs` contained a commented-out Bard session cookie. Treat it as leaked and revoke it.
- Legacy read Gate.io/XT keys from user environment variables and sent signed orders. Anything you add back should read secrets from env/user-secrets only, never source, and sit behind a separate `IBroker` abstraction with an explicit live-trading opt-in.

## Known gaps
- Gemini `v2/candles` behavior (row order, row cap per call, whether the newest row is still forming, volume currency) is coded from the API reference page and fixtures, not exercised live. The client sorts rows, drops the forming candle by time arithmetic, and cannot page: `--count` above what Gemini returns yields fewer candles.
- No `Microsoft.Extensions.*` hosting/DI/logging or xunit: NuGet was unreachable when this was built. Moving to Generic Host + `ILogger` + xunit is mechanical.
