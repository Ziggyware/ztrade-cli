# Interactive charting

`ztrade chart` writes a self-contained HTML chart; it does not require a web server, browser-side network access, or a third-party charting CDN.

```sh
ztrade chart --symbol BTCUSD --interval 1hr --count 1000 --out btcusd.html
ztrade chart --csv candles.csv --symbol BTCUSD --interval 1hr --out btcusd.html
ztrade chart --csv candles.csv --out full.html --full   # full client app with 60 ultra studies
```

Open the generated HTML file in a modern browser. The chart includes the existing pattern scanner's matches, candle and volume rendering, a pattern ledger, and 60 selectable studies (50 base + 10 ultra indicative). The loaded dataset is preserved in the export. Add `--full` (aliases: `--client`, `--full-client`, `--full-app`, `--app`, `--show-full-client`) to enable the **full client app** view with an elevated top-bar badge, extra stats, and the 10 ultra indicative overlays/panels auto-highlighted.

## Navigation

- Scroll over the chart to zoom around the pointer; drag to pan.
- Hover updates the in-chart OHLCV ribbon and a detailed tooltip with candle return, relative volume, RSI, ATR, CMF, volume-delta proxy, and any pattern matches on that bar. A compact color legend tracks the active overlays and studies.
- Use the 50/100/250/500/All window buttons, `+` / `−`, or double-click / **Fit all**. Arrow keys pan one candle; Shift+Arrow pans ten; `+`, `-`, and `F` zoom or fit.
- Switch between regular, hollow, Heikin–Ashi, and close-line views. The tooltip always reports the original source OHLCV.
- Click a pattern-ledger row to focus that match. Export PNG saves the current canvas at device-pixel-ratio resolution (up to 4×).
- For anchored VWAP, select **Anchor VWAP**, then click a candle. For an R-multiple ruler, enable that study, drag entry-to-stop, release, then click a target price.

## Study catalogue (60) — 50 base + 10 ultra indicative

### Structure and price action

1. All candlestick-pattern matches returned by `PatternScanner`.
2. Confirmed three-left / three-right swing pivots.
3. Heuristic break-of-structure (BOS) and change-of-character (CHoCH) events.
4. Equal-high / equal-low pivot clusters within 0.15 ATR.
5. Wick-through-and-close-back liquidity-sweep proxies.
6. Three-candle imbalance / fair-value-gap zones, terminated at first later overlap.
7. Candidate order-block zones (last opposing candle before a heuristic break).
8. Candidate breaker marks after a close invalidates an order-block range.
9. Candle opens outside the previous candle range.
10. 0.382, 0.500, 0.618, and 0.786 retracement rails for the latest swing pair.

### Price overlays

11. SMA(20). 12. EMA(20). 13. EMA(50). 14. UTC-day VWAP proxy. 15. Click-anchored VWAP proxy. 16. Bollinger envelope (20, 2σ). 17. Keltner channel (EMA 20 ± 1.5 ATR). 18. Donchian channel (20). 19. Supertrend (10, 3 ATR). 20. Ichimoku Tenkan(9) and Kijun(26) lines (unshifted).
21. **ULTRA** Hull Moving Average HMA(21) — responsive trend filter (WMA(2×WMA(n/2)−WMA(n)) smoothed by WMA(sqrt(n))).
22. **ULTRA** Kaufman Adaptive MA KAMA(10,2,30) — efficiency-ratio adaptive trend.
23. **ULTRA** Parabolic SAR(0.02,0.20) — trailing stop & reversal dots.

### Volume and money-flow proxies

24. Candle-body-direction-signed volume delta proxy. 25. Cumulative delta proxy. 26. On-balance volume. 27. Chaikin money flow (20). 28. Money Flow Index (14). 29. Accumulation/distribution line. 30. Smoothed Force Index (EMA 13). 31. Volume-price trend. 32. Relative volume (20). 33. Visible-range volume profile. 34. Approximate POC and contiguous 70% value area. 35. Volume-weighted moving average (20). 36. Amihud-style price-impact proxy in basis points per million notional.

### Momentum and volatility

37. RSI (14). 38. MACD (12, 26, 9). 39. Stochastic (14, 3). 40. ADX and directional indices (14). 41. ATR (14). 42. CCI (20). 43. Rolling 20-bar realized log-return volatility. 44. Parkinson high/low volatility estimate (20). 45. Close drawdown from the dataset's running peak. 46. Close z-score (20).
47. **ULTRA** Aroon Up/Down (14) — time since high/low.
48. **ULTRA** Williams %R (14) — momentum extremes (-20/-80).
49. **ULTRA** Rate of Change ROC(12) — pure velocity.
50. **ULTRA** Stochastic RSI (14,14,3,3) — amplifies RSI extremes.
51. **ULTRA** Bollinger Bandwidth & %B (20,2σ) — squeeze & position in bands.
52. **ULTRA** Coppock Curve (11,14,10 WMA) — long-term bottom / institutional accumulation signal.
53. **ULTRA** Elder Impulse System (EMA13 + MACD hist) — green/red/blue impulse gauge.

### Risk and context

54. Close ± 2 ATR reference rails. 55. Latest confirmed pivot support/resistance. 56. Two-stage entry/stop/target R-multiple ruler. 57. Risk-budget position-size calculator. 58. Post-pivot maximum favorable/adverse excursion readout. 59. ADX / relative-volatility trend-range regime heuristic. 60. Broad UTC session shading (00–08 / 07–16 / 13–22 UTC; overlapping hours use the first matching bucket).

Add `--full` to highlight all ultra studies in the legend and set the full-client badge to "FULL CLIENT APP · 60 ULTRA STUDIES". Without the flag the ultra studies remain toggleable but the default active set is the original 9; with the flag the top-bar shows the full client chrome and enables 6 of the ultra studies by default.

## Full client app flag

```
--full            enable full client app (recommended)
--client          alias for --full
--full-client     alias for --full
--full-app        alias for --full
--app             alias for --full
--show-full-client alias for --full
```

All aliases set the same `fullClient` field in the exported JSON (`DATA.fullClient`). The view reads this flag to apply the full chrome and to count 60 studies.

## Data and interpretation limits

This CLI currently exports candles (OHLCV) and pattern matches; it does not receive individual executions, aggressor-side flags, bid/ask quotes, order-book depth, funding, or liquidation events. Therefore candle-direction delta/CVD, VWAP, profile, volume-at-price, sweeps, blocks, and session marks are explicitly estimates or heuristics. The volume profile allocates each candle's volume across its high-low envelope, not across observed trades at price. These studies are descriptive, not proof of actual capital flow, predictive edge, or a recommendation to trade. Do not treat the sizing widget, ATR rails, or pattern marks as financial advice.

True order-flow visualization needs timestamped trades with aggressor side and/or depth snapshots from a data source that provides them; those data are not fabricated from candles here.

