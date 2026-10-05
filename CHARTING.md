# Interactive charting

`ztrade chart` writes a self-contained HTML chart; it does not require a web server, browser-side network access, or a third-party charting CDN.

```sh
ztrade chart --symbol BTCUSD --interval 1hr --count 1000 --out btcusd.html
ztrade chart --csv candles.csv --symbol BTCUSD --interval 1hr --out btcusd.html
```

Open the generated HTML file in a modern browser. The chart includes the existing pattern scanner's matches, candle and volume rendering, a pattern ledger, and 50 selectable studies. The loaded dataset is preserved in the export.

## Navigation

- Scroll over the chart to zoom around the pointer; drag to pan.
- Hover updates the in-chart OHLCV ribbon and a detailed tooltip with candle return, relative volume, RSI, ATR, CMF, volume-delta proxy, and any pattern matches on that bar. A compact color legend tracks the active overlays and studies.
- Use the 50/100/250/500/All window buttons, `+` / `−`, or double-click / **Fit all**. Arrow keys pan one candle; Shift+Arrow pans ten; `+`, `-`, and `F` zoom or fit.
- Switch between regular, hollow, Heikin–Ashi, and close-line views. The tooltip always reports the original source OHLCV.
- Click a pattern-ledger row to focus that match. Export PNG saves the current canvas at device-pixel-ratio resolution (up to 4×).
- For anchored VWAP, select **Anchor VWAP**, then click a candle. For an R-multiple ruler, enable that study, drag entry-to-stop, release, then click a target price.

## Study catalogue (50)

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

### Volume and money-flow proxies

21. Candle-body-direction-signed volume delta proxy. 22. Cumulative delta proxy. 23. On-balance volume. 24. Chaikin money flow (20). 25. Money Flow Index (14). 26. Accumulation/distribution line. 27. Smoothed Force Index (EMA 13). 28. Volume-price trend. 29. Relative volume (20). 30. Visible-range volume profile. 31. Approximate POC and contiguous 70% value area. 32. Volume-weighted moving average (20). 33. Amihud-style price-impact proxy in basis points per million notional.

### Momentum and volatility

34. RSI (14). 35. MACD (12, 26, 9). 36. Stochastic (14, 3). 37. ADX and directional indices (14). 38. ATR (14). 39. CCI (20). 40. Rolling 20-bar realized log-return volatility. 41. Parkinson high/low volatility estimate (20). 42. Close drawdown from the dataset's running peak. 43. Close z-score (20).

### Risk and context

44. Close ± 2 ATR reference rails. 45. Latest confirmed pivot support/resistance. 46. Two-stage entry/stop/target R-multiple ruler. 47. Risk-budget position-size calculator. 48. Post-pivot maximum favorable/adverse excursion readout. 49. ADX / relative-volatility trend-range regime heuristic. 50. Broad UTC session shading (00–08 / 07–16 / 13–22 UTC; overlapping hours use the first matching bucket).

## Data and interpretation limits

This CLI currently exports candles (OHLCV) and pattern matches; it does not receive individual executions, aggressor-side flags, bid/ask quotes, order-book depth, funding, or liquidation events. Therefore candle-direction delta/CVD, VWAP, profile, volume-at-price, sweeps, blocks, and session marks are explicitly estimates or heuristics. The volume profile allocates each candle's volume across its high-low envelope, not across observed trades at price. These studies are descriptive, not proof of actual capital flow, predictive edge, or a recommendation to trade. Do not treat the sizing widget, ATR rails, or pattern marks as financial advice.

True order-flow visualization needs timestamped trades with aggressor side and/or depth snapshots from a data source that provides them; those data are not fabricated from candles here.
