namespace ZTrade.Cli;

internal static class ChartPage
{
    public const string Html = """
<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta name="color-scheme" content="dark">
<title>ztrade · Flow Lens</title>
<style>
:root{color-scheme:dark;--bg:#071116;--surface:#0d1a20;--surface2:#102129;--line:#1d3239;--muted:#82979d;--text:#e3eff0;--green:#42d6a0;--red:#ff687a;--cyan:#48c8d8;--amber:#f4bd62;--purple:#b49cff;--blue:#76a8ff;--shadow:0 12px 34px #0005}
*{box-sizing:border-box}html,body{height:100%;margin:0;background:var(--bg);color:var(--text);font-family:Inter,ui-sans-serif,system-ui,-apple-system,"Segoe UI",sans-serif}button,input,select{font:inherit;color:inherit}button{border:1px solid var(--line);background:#102027;border-radius:8px;padding:7px 10px;cursor:pointer;transition:.15s ease}button:hover{border-color:#38606b;background:#15313a}button:focus-visible,input:focus-visible,select:focus-visible{outline:2px solid var(--cyan);outline-offset:2px}.topbar{height:62px;padding:0 18px;display:flex;align-items:center;gap:16px;border-bottom:1px solid var(--line);background:#09151a}.brand{display:flex;align-items:center;gap:10px;min-width:190px}.brand-mark{width:30px;height:30px;border-radius:9px;background:linear-gradient(135deg,#40d9a4,#3994a8);display:grid;place-items:center;color:#052019;font-weight:900;font-size:14px}.brand-name{font-weight:800;letter-spacing:.02em}.brand-sub{color:var(--muted);font-size:10px;letter-spacing:.18em;text-transform:uppercase;margin-top:2px}.instrument{border-left:1px solid var(--line);padding-left:17px;min-width:145px}.instrument strong{font-size:14px;letter-spacing:.05em}.instrument span{display:block;color:var(--muted);font-size:11px;margin-top:2px}.top-spacer{flex:1}.source-badge{border:1px solid #6b5630;color:#ffd68b;background:#6d4b171c;border-radius:999px;padding:6px 9px;font-size:10px;white-space:nowrap;letter-spacing:.03em}.top-actions{display:flex;gap:7px;align-items:center}.top-actions button{font-size:11px;padding:7px 9px}.layout{height:calc(100% - 62px);display:grid;grid-template-columns:minmax(0,1fr) 322px;min-height:0}.main{min-width:0;min-height:0;padding:12px 14px 12px 16px;display:flex;flex-direction:column;gap:10px}.toolbar{min-height:38px;display:flex;align-items:center;gap:7px;flex-wrap:wrap}.toolbar .divider{height:23px;width:1px;background:var(--line);margin:0 3px}.toolbar button{padding:6px 9px;font-size:11px}.toolbar .selected{border-color:#338371;color:#86ebc4;background:#12352d}.toolbar select{background:#102027;border:1px solid var(--line);border-radius:8px;padding:6px 24px 6px 9px;font-size:11px}.toolbar-label{color:var(--muted);font-size:10px;margin-left:2px}.statline{display:flex;gap:8px;overflow-x:auto;padding-bottom:1px;scrollbar-width:thin}.stat{min-width:120px;border:1px solid var(--line);border-radius:9px;padding:8px 10px;background:linear-gradient(145deg,#102027,#0b171c);flex:1}.stat span{display:block;color:var(--muted);font-size:9px;letter-spacing:.12em;text-transform:uppercase;white-space:nowrap}.stat b{display:block;font-size:13px;font-variant-numeric:tabular-nums;margin-top:4px;white-space:nowrap}.positive{color:var(--green)!important}.negative{color:var(--red)!important}.chart-shell{position:relative;flex:1;min-height:420px;border:1px solid var(--line);border-radius:12px;overflow:hidden;background:#081319;box-shadow:var(--shadow)}#chart{display:block;width:100%;height:100%;touch-action:none;cursor:crosshair}.tooltip{position:absolute;display:none;z-index:5;pointer-events:none;min-width:220px;max-width:300px;background:#0a171eec;border:1px solid #35515a;border-radius:9px;padding:10px 12px;box-shadow:var(--shadow);font-size:11px;backdrop-filter:blur(10px)}.tooltip-head{display:flex;justify-content:space-between;gap:10px;border-bottom:1px solid var(--line);padding-bottom:6px;margin-bottom:6px;font-weight:700}.tooltip-grid{display:grid;grid-template-columns:1fr 1fr;gap:4px 14px;color:var(--muted)}.tooltip-grid b{float:right;color:var(--text);font-weight:600;font-variant-numeric:tabular-nums}.tooltip-patterns{margin-top:7px;color:var(--cyan);line-height:1.4;max-height:72px;overflow:auto}.chart-foot{min-height:18px;display:flex;align-items:center;justify-content:space-between;color:var(--muted);font-size:10px}.chart-foot strong{color:#cce0e2;font-weight:600}.sidebar{min-width:0;min-height:0;border-left:1px solid var(--line);background:#0a161b;display:flex;flex-direction:column}.side-head{padding:14px 14px 11px;border-bottom:1px solid var(--line)}.side-head-row{display:flex;align-items:center;justify-content:space-between}.side-head h2{font-size:12px;text-transform:uppercase;letter-spacing:.12em;margin:0}.feature-count{font-size:10px;color:var(--cyan);font-weight:700}.side-sub{color:var(--muted);font-size:10px;line-height:1.45;margin:6px 0 0}.search-wrap{padding:10px 12px 8px}.search-wrap input{width:100%;background:#0e2026;border:1px solid var(--line);border-radius:8px;padding:8px 10px;font-size:11px}.feature-list{overflow:auto;padding:0 8px 8px;flex:1;scrollbar-color:#294149 transparent;scrollbar-width:thin}.group-title{padding:10px 7px 5px;color:#6f9299;font-size:9px;letter-spacing:.14em;text-transform:uppercase;position:sticky;top:0;background:#0a161bf2;z-index:1}.feature{display:flex;gap:8px;align-items:flex-start;padding:6px 7px;border-radius:7px;cursor:pointer}.feature:hover{background:#102229}.feature input{accent-color:#4fd1a0;margin:2px 0 0;width:13px;height:13px;flex:0 0 auto}.feature-copy{min-width:0}.feature-name{font-size:10px;line-height:1.25}.feature-note{font-size:9px;color:var(--muted);line-height:1.35;margin-top:2px}.feature-empty{padding:16px;color:var(--muted);font-size:11px}.ledger{border-top:1px solid var(--line);max-height:235px;overflow:auto;padding:10px 12px 12px;scrollbar-width:thin}.ledger-title{display:flex;justify-content:space-between;align-items:center;font-size:10px;text-transform:uppercase;letter-spacing:.11em;color:#b7c9cd;font-weight:700}.ledger-title span{color:var(--cyan);letter-spacing:0}.pattern-types{display:flex;flex-wrap:wrap;gap:4px;margin:8px 0}.pattern-type{border:1px solid #243d44;color:#a8bec2;border-radius:99px;padding:3px 6px;font-size:9px}.pattern-type b{color:#fff;margin-left:3px}.pattern-row{display:flex;gap:8px;justify-content:space-between;padding:5px 1px;border-bottom:1px solid #17282e;font-size:9px;cursor:pointer}.pattern-row:hover{color:var(--cyan)}.pattern-row span:first-child{overflow:hidden;text-overflow:ellipsis;white-space:nowrap}.pattern-row time{color:var(--muted);white-space:nowrap}.warning{margin:0 12px 10px;border:1px solid #584522;border-radius:8px;padding:8px 9px;background:#5b431418;color:#d9c99f;font-size:9px;line-height:1.45}.size-card{padding:10px 12px;border-top:1px solid var(--line);display:none}.size-card.visible{display:block}.size-title{font-size:10px;font-weight:700;text-transform:uppercase;letter-spacing:.1em;margin-bottom:8px}.size-grid{display:grid;grid-template-columns:1fr 1fr;gap:7px}.size-grid label{font-size:9px;color:var(--muted)}.size-grid input{display:block;width:100%;margin-top:4px;background:#0e2026;border:1px solid var(--line);border-radius:6px;padding:6px;font-size:10px}.size-result{margin-top:8px;border-radius:7px;background:#102027;padding:8px;font-size:10px;line-height:1.6;color:#a5bdc1}.size-result strong{color:var(--green)}.mobile-tools{display:none}.pill{border:1px solid var(--line);border-radius:999px;padding:3px 7px;font-size:9px;color:#a4b9bd}.empty-canvas{position:absolute;inset:0;display:none;align-items:center;justify-content:center;color:var(--muted);pointer-events:none}.hide{display:none!important}
.full-client-badge{border:1px solid #2e7a6a;color:#86ebc4;background:linear-gradient(135deg,#0e2a22,#12352d);border-radius:999px;padding:6px 10px;font-size:10px;letter-spacing:.06em;font-weight:700;white-space:nowrap}.full-client-badge.full{background:#12352d;border-color:#42d6a0;box-shadow:0 0 12px #42d6a055}
.chart-shell{isolation:isolate;border-color:#24343d;border-radius:14px;background:radial-gradient(ellipse at 50% 5%,#10202a 0%,#081319 54%);box-shadow:0 22px 58px #0006,inset 0 1px #ffffff08}.chart-head{position:absolute;z-index:2;top:11px;left:17px;right:15px;display:flex;flex-direction:column;gap:7px;pointer-events:none}.chart-readout{display:flex;align-items:center;gap:13px;min-width:0;overflow:hidden;white-space:nowrap;font:10px ui-monospace,SFMono-Regular,Menlo,monospace;color:#81949e}.bar-time{color:#e4edf3;font-weight:700;letter-spacing:.025em;margin-right:3px}.ohlc-item{display:inline-flex;gap:4px;align-items:center}.ohlc-item b{color:#d7e0e7;font-weight:600}.ohlc-item.positive b{color:var(--green)}.ohlc-item.negative b{color:var(--red)}.chart-legend{display:flex;align-items:center;gap:6px;overflow:hidden;height:18px;white-space:nowrap}.legend-chip{display:inline-flex;align-items:center;gap:5px;height:18px;border:1px solid #ffffff0d;border-radius:5px;padding:0 6px;background:#101c24b8;color:#a6b5bd;font-size:9px;letter-spacing:.02em}.legend-dot{width:6px;height:6px;border-radius:50%;background:var(--legend-color);box-shadow:0 0 7px color-mix(in srgb,var(--legend-color),transparent 45%)}.legend-more{color:#7f939c;font-size:9px;padding-left:2px}.stat{position:relative;overflow:hidden;border-color:#22323a;background:linear-gradient(145deg,#101e26,#0a141a);box-shadow:inset 0 1px #ffffff06}.stat::after{content:"";position:absolute;left:0;top:0;width:2px;height:100%;background:linear-gradient(180deg,#56d8b0,#3d8e9d);opacity:.55}.stat span{letter-spacing:.13em}.toolbar button,.toolbar select{box-shadow:inset 0 1px #ffffff05}.toolbar button:hover{transform:translateY(-1px)}.feature{transition:background .14s ease,padding-left .14s ease}.feature:hover{padding-left:10px}.feature input{accent-color:#58d6a7}.pattern-row{transition:color .12s ease,background .12s ease}.pattern-row:hover{background:#102229}.tooltip{border-color:#45606a;background:#09151df2;box-shadow:0 12px 40px #0009}
@media(max-width:1100px){.layout{grid-template-columns:minmax(0,1fr) 292px}.brand{min-width:150px}.source-badge{display:none}}
@media(max-width:850px){.topbar{padding:0 10px;height:56px;gap:10px}.layout{height:calc(100% - 56px);grid-template-columns:1fr}.main{padding:9px}.sidebar{position:fixed;z-index:20;right:0;top:56px;bottom:0;width:min(340px,92vw);box-shadow:var(--shadow);transform:translateX(102%);transition:transform .2s ease}.sidebar.open{transform:translateX(0)}.mobile-tools{display:inline-flex}.brand{min-width:auto}.instrument{padding-left:10px;min-width:0}.instrument span{display:none}.stat{min-width:105px}.chart-shell{min-height:350px}.top-actions .hide-mobile{display:none}}
@media(max-width:500px){.brand-sub{display:none}.brand-name{font-size:12px}.brand-mark{width:27px;height:27px}.instrument strong{font-size:12px}.main{gap:7px}.toolbar{gap:4px}.toolbar button,.toolbar select{padding:6px 7px;font-size:10px}.chart-shell{min-height:320px}.stat{min-width:96px;padding:7px 8px}.stat b{font-size:11px}}
@media(max-width:600px){.chart-head{left:10px;right:10px;top:9px;gap:5px}.chart-readout{gap:8px;font-size:8px}.chart-legend{gap:4px}.legend-chip{font-size:8px;padding:0 5px}}
@media(max-width:420px){.bar-time{display:none}.ohlc-item:nth-child(6){display:none}.chart-head{left:8px;right:8px}}
</style>
</head>
<body>
<header class="topbar">
  <div class="brand"><div class="brand-mark">Z</div><div><div class="brand-name">ztrade</div><div class="brand-sub">Flow Lens</div></div></div>
  <div class="instrument"><strong id="symbol">—</strong><span id="interval">—</span></div>
  <div class="top-spacer"></div>
  <div class="source-badge" title="Candle data cannot identify aggressor side or order-book liquidity">OHLCV FLOW PROXIES · NOT TAPE DATA</div>
  <div class="full-client-badge" id="fullClientBadge" style="display:none">FULL CLIENT APP · 60 ULTRA STUDIES</div>
  <div class="top-actions"><button id="openFeatures" class="mobile-tools" aria-label="Open chart studies">Studies · 60</button><button id="exportPng" class="hide-mobile" title="Save the current high-resolution canvas as PNG">Export PNG</button><button id="fitTop" class="hide-mobile" title="Fit all loaded candles">Fit all</button></div>
</header>
<div class="layout">
  <main class="main">
    <div class="toolbar" aria-label="Chart controls">
      <span class="toolbar-label">VIEW</span>
      <select id="viewMode" aria-label="Chart rendering mode"><option value="candles">Candles</option><option value="hollow">Hollow candles</option><option value="heikin">Heikin–Ashi</option><option value="line">Close line</option></select>
      <span class="divider"></span>
      <span class="toolbar-label">WINDOW</span>
      <button data-window="50">50</button><button data-window="100">100</button><button data-window="250">250</button><button data-window="500">500</button><button data-window="all">All</button>
      <span class="divider"></span>
      <button id="zoomOut" title="Zoom out">−</button><button id="zoomIn" title="Zoom in">+</button>
      <button id="anchorButton" title="Click a candle to anchor the anchored VWAP">Anchor VWAP</button>
      <button id="exportPngSmall" class="mobile-tools" title="Save chart as PNG">PNG</button>
    </div>
    <div class="statline" aria-live="polite">
      <div class="stat"><span>Last candle close</span><b id="statPrice">—</b></div>
      <div class="stat"><span>Visible change</span><b id="statChange">—</b></div>
      <div class="stat"><span>Relative volume · 20</span><b id="statRvol">—</b></div>
      <div class="stat"><span>VWAP distance</span><b id="statVwap">—</b></div>
      <div class="stat"><span>20-bar delta proxy</span><b id="statDelta">—</b></div>
      <div class="stat"><span>Detected patterns</span><b id="statPatterns">—</b></div>
    </div>
    <div class="chart-shell" id="chartShell">
      <div class="chart-head">
        <div class="chart-readout">
          <span class="bar-time" id="barTime">LATEST · —</span>
          <span class="ohlc-item">O <b id="readOpen">—</b></span>
          <span class="ohlc-item">H <b id="readHigh">—</b></span>
          <span class="ohlc-item">L <b id="readLow">—</b></span>
          <span class="ohlc-item">C <b id="readClose">—</b></span>
          <span class="ohlc-item">VOL <b id="readVolume">—</b></span>
          <span class="ohlc-item" id="readChangeWrap">Δ <b id="readChange">—</b></span>
        </div>
        <div class="chart-legend" id="chartLegend" aria-label="Visible series"></div>
      </div>
      <canvas id="chart" aria-label="Interactive candlestick chart. Use wheel to zoom, drag to pan, and hover a candle for details."></canvas>
      <div class="tooltip" id="tooltip" role="status"></div>
      <div class="empty-canvas" id="emptyMessage">No candle data was included.</div>
    </div>
    <div class="chart-foot"><span id="footStatus">Wheel to zoom · drag to pan · hover for OHLCV · double-click to fit</span><span><strong id="regimeBadge"></strong> <span id="paneBadge"></span></span></div>
  </main>
  <aside class="sidebar" id="sidebar">
    <div class="side-head"><div class="side-head-row"><h2>Advanced studies</h2><span class="feature-count" id="featureCount">0 / 60 active</span></div><p class="side-sub">Sixty toggleable price, structure, volume-flow, volatility and risk lenses. “Proxy” means inferred from OHLCV candles.</p></div>
    <div class="search-wrap"><input id="featureSearch" type="search" placeholder="Find a study…" aria-label="Search chart studies"></div>
    <div class="feature-list" id="featureList"></div>
    <section class="size-card" id="sizeCard" aria-label="Position size calculator">
      <div class="size-title">Risk-size calculator <span class="pill">illustrative</span></div>
      <div class="size-grid">
        <label>Account value<input id="sizeEquity" type="number" min="0" step="100" value="10000"></label>
        <label>Risk %<input id="sizeRisk" type="number" min="0" step="0.1" value="1"></label>
        <label>Entry price<input id="sizeEntry" type="number" min="0" step="any"></label>
        <label>Stop distance<input id="sizeDistance" type="number" min="0" step="any"></label>
      </div>
      <div class="size-result" id="sizeResult">Enter a positive stop distance.</div>
    </section>
    <section class="ledger" aria-label="Pattern ledger"><div class="ledger-title">Pattern ledger <span id="ledgerTotal">0 matches</span></div><div class="pattern-types" id="patternTypes"></div><div id="patternRows"></div></section>
    <p class="warning"><strong>Data limit:</strong> no trades, bid/ask aggressor flags, funding, liquidations or L2 book are supplied here. Delta/CVD, VWAP, profile and liquidity marks are candle-derived approximations—not actual money flow or a profit signal.</p>
  </aside>
</div>
<script id="chart-data" type="application/json">__ZTRADE_DATA__</script>
<script>
'use strict';
const DATA = JSON.parse(document.getElementById('chart-data').textContent);
const C = (DATA.candles || []).map((x, i) => ({
  i, time: Date.parse(x.time), open: +x.open, high: +x.high, low: +x.low, close: +x.close, volume: +x.volume
}));
const N = C.length;
const PATTERNS = (DATA.patterns || []).map(p => ({start:+p.start, end:+p.end, type:String(p.type), bias:String(p.bias)}));
const chart = document.getElementById('chart');
const ctx = chart.getContext('2d', {alpha:false});
const shell = document.getElementById('chartShell');
const tooltip = document.getElementById('tooltip');
const FULL_CLIENT_DEFAULTS = ['hma21','kama','aroon','willr','bbWidth','coppock'];
const ACTIVE_DEFAULTS = ['patterns','swing-pivots','sweeps','fvg','vwap','volume-profile','profile-value','support','regime'];
const active = new Set(DATA.fullClient || DATA.full || DATA.client ? [...ACTIVE_DEFAULTS, ...FULL_CLIENT_DEFAULTS] : ACTIVE_DEFAULTS);
const COLORS = {green:'#42d6a0',red:'#ff687a',cyan:'#48c8d8',amber:'#f4bd62',purple:'#b49cff',blue:'#76a8ff',muted:'#82979d',grid:'#172b31',text:'#92a9ae',bg:'#081319'};
const FEATURES = [
 {id:'patterns',group:'Structure & price action',name:'All detected candlestick patterns',mode:'marks',note:'Every match from ztrade’s pattern scanner; hover or click a ledger row.'},
 {id:'swing-pivots',group:'Structure & price action',name:'Confirmed swing pivots',mode:'marks',note:'Three-bar left/right pivot confirmation; pivots appear with a small delay.'},
 {id:'structure',group:'Structure & price action',name:'BOS / CHoCH structure breaks',mode:'marks',note:'Close-through of the latest confirmed pivot; heuristic structure labels.'},
 {id:'equal-levels',group:'Structure & price action',name:'Equal highs / equal lows',mode:'marks',note:'Nearby confirmed pivots clustered within 0.15 ATR.'},
 {id:'sweeps',group:'Structure & price action',name:'Liquidity-sweep wick proxies',mode:'marks',note:'Price pierces a pivot then closes back across it; not resting-order data.'},
 {id:'fvg',group:'Structure & price action',name:'Three-candle imbalance zones',mode:'zones',note:'OHLC fair-value-gap proxy; zones stop at the first later overlap.'},
 {id:'order-blocks',group:'Structure & price action',name:'Candidate order-block zones',mode:'zones',note:'Last opposing candle before a heuristic structure break.'},
 {id:'breaker-blocks',group:'Structure & price action',name:'Candidate breaker blocks',mode:'marks',note:'Candidate blocks invalidated by a later close through their range.'},
 {id:'gaps',group:'Structure & price action',name:'Open / range gaps',mode:'marks',note:'Open outside the prior candle range; session gap context is not inferred.'},
 {id:'fib',group:'Structure & price action',name:'Latest-swing Fibonacci rails',mode:'levels',note:'0.382 / 0.5 / 0.618 / 0.786 retracements from the latest pivot pair.'},
 {id:'sma20',group:'Price overlays',name:'Simple moving average · 20',mode:'overlay',note:'Close-based SMA(20).'},
 {id:'ema20',group:'Price overlays',name:'Exponential moving average · 20',mode:'overlay',note:'Close-based EMA(20).'},
 {id:'ema50',group:'Price overlays',name:'Exponential moving average · 50',mode:'overlay',note:'Close-based EMA(50).'},
 {id:'vwap',group:'Price overlays',name:'UTC-day VWAP proxy',mode:'overlay',note:'Typical-price × candle-volume, reset at UTC date boundaries.'},
 {id:'avwap',group:'Price overlays',name:'Click-anchored VWAP proxy',mode:'overlay',note:'Arm the toolbar control, then click a candle to choose its anchor.'},
 {id:'bollinger',group:'Price overlays',name:'Bollinger envelope · 20, 2σ',mode:'overlay',note:'Rolling close mean and standard deviation.'},
 {id:'keltner',group:'Price overlays',name:'Keltner channel · 20, 1.5 ATR',mode:'overlay',note:'EMA center with an ATR envelope.'},
 {id:'donchian',group:'Price overlays',name:'Donchian channel · 20',mode:'overlay',note:'Rolling highs and lows, including the current candle.'},
 {id:'supertrend',group:'Price overlays',name:'Supertrend · 10, 3 ATR',mode:'overlay',note:'ATR trailing trend line; parameterized heuristic, not a forecast.'},
 {id:'ichimoku',group:'Price overlays',name:'Ichimoku conversion / base',mode:'overlay',note:'Tenkan(9) and Kijun(26); values are not shifted forward.'},
 {id:'delta',group:'Volume & money-flow proxies',name:'Candle-direction volume delta proxy',mode:'panel',note:'Volume signed by candle body direction; no buyer/seller aggressor data.'},
 {id:'cvd',group:'Volume & money-flow proxies',name:'Cumulative delta proxy',mode:'panel',note:'Running sum of candle-direction volume delta; reset at dataset start.'},
 {id:'obv',group:'Volume & money-flow proxies',name:'On-balance volume',mode:'panel',note:'Volume signed by close-to-close direction.'},
 {id:'cmf',group:'Volume & money-flow proxies',name:'Chaikin money flow · 20',mode:'panel',note:'Close-location value weighted by reported candle volume.'},
 {id:'mfi',group:'Volume & money-flow proxies',name:'Money Flow Index · 14',mode:'panel',note:'Typical-price money-flow ratio; candle-volume input.'},
 {id:'adl',group:'Volume & money-flow proxies',name:'Accumulation / distribution line',mode:'panel',note:'Cumulative close-location-value × volume.'},
 {id:'force',group:'Volume & money-flow proxies',name:'Force Index · EMA 13',mode:'panel',note:'Close change × candle volume, smoothed.'},
 {id:'vpt',group:'Volume & money-flow proxies',name:'Volume-price trend',mode:'panel',note:'Cumulative percent close change × volume.'},
 {id:'rvol',group:'Volume & money-flow proxies',name:'Relative volume · 20',mode:'panel',note:'Current candle volume divided by its rolling 20-bar average.'},
 {id:'volume-profile',group:'Volume & money-flow proxies',name:'Visible-range volume profile',mode:'profile',note:'Range-overlap allocation of each candle’s volume; not tick-level volume-at-price.'},
 {id:'profile-value',group:'Volume & money-flow proxies',name:'Profile POC + 70% value area',mode:'levels',note:'Approximate point of control, value-area high and low for visible bars.'},
 {id:'vwma',group:'Volume & money-flow proxies',name:'Volume-weighted moving average · 20',mode:'overlay',note:'Rolling close × volume divided by rolling volume.'},
 {id:'amihud',group:'Volume & money-flow proxies',name:'Amihud price-impact proxy',mode:'panel',note:'Absolute log return per million notional, in basis points.'},
 {id:'rsi',group:'Momentum & volatility',name:'Relative Strength Index · 14',mode:'panel',note:'Wilder-smoothed close momentum; 30/70 guides.'},
 {id:'macd',group:'Momentum & volatility',name:'MACD · 12, 26, 9',mode:'panel',note:'EMA spread, signal and histogram.'},
 {id:'stochastic',group:'Momentum & volatility',name:'Stochastic · 14, 3',mode:'panel',note:'Rolling range position with %K / %D.'},
 {id:'adx',group:'Momentum & volatility',name:'ADX / directional index · 14',mode:'panel',note:'Smoothed directional movement; includes +DI and −DI.'},
 {id:'atr',group:'Momentum & volatility',name:'Average True Range · 14',mode:'panel',note:'Wilder-smoothed true range in price units.'},
 {id:'cci',group:'Momentum & volatility',name:'Commodity Channel Index · 20',mode:'panel',note:'Typical-price deviation from its rolling mean.'},
 {id:'realized-vol',group:'Momentum & volatility',name:'20-bar realized volatility',mode:'panel',note:'Rolling standard deviation of log returns; not annualized.'},
 {id:'parkinson',group:'Momentum & volatility',name:'Parkinson range volatility · 20',mode:'panel',note:'High/low range estimator; assumes continuous diffusion.'},
 {id:'drawdown',group:'Momentum & volatility',name:'Running-peak drawdown',mode:'panel',note:'Close relative to the maximum close seen so far in this file.'},
 {id:'zscore',group:'Momentum & volatility',name:'Close z-score · 20',mode:'panel',note:'Close deviation from rolling mean in standard deviations.'},
 {id:'atr-stops',group:'Risk & context',name:'Two-ATR risk rails',mode:'overlay',note:'Close ± 2 ATR reference rails, not suggested stop orders.'},
 {id:'support',group:'Risk & context',name:'Latest pivot support / resistance',mode:'levels',note:'Most recent confirmed low and high pivots.'},
 {id:'ruler',group:'Risk & context',name:'Interactive R-multiple ruler',mode:'tool',note:'Drag entry to stop, then click a target price to measure directional R multiple.'},
 {id:'sizing',group:'Risk & context',name:'Risk-budget position-size calculator',mode:'calculator',note:'Quantity = risk budget / stop distance; excludes fees, leverage and slippage.'},
 {id:'excursions',group:'Risk & context',name:'Post-pivot MFE / MAE gauge',mode:'readout',note:'Observed high/low excursion after the latest confirmed pivot.'},
 {id:'regime',group:'Risk & context',name:'Trend / range / volatility regime',mode:'readout',note:'ADX plus relative realized volatility; a descriptive heuristic.'},
 {id:'sessions',group:'Risk & context',name:'UTC session shading',mode:'shade',note:'Broad 00–08 / 07–16 / 13–22 UTC windows; overlap is intentional.'},
 {id:'hma21',group:'Ultra trend & adaptive',name:'Hull Moving Average · 21 — ultra responsive',mode:'overlay',note:'Ultra indicative: HMA(21) via WMA(2*WMA(n/2)-WMA(n), sqrt(n)); reduces lag while smoothing.'},
 {id:'kama',group:'Ultra trend & adaptive',name:'Kaufman Adaptive MA · 10,2,30 — noise-aware',mode:'overlay',note:'Ultra indicative: KAMA adapts to efficiency ratio; fast in trend, slow in noise.'},
 {id:'psar',group:'Ultra trend & adaptive',name:'Parabolic SAR · 0.02/0.20 — trailing stops',mode:'overlay',note:'Ultra indicative: SAR dots flip on trend reversal; classic trailing stop.'},
 {id:'aroon',group:'Ultra momentum & squeeze',name:'Aroon · 14 — time-since-high/low',mode:'panel',note:'Ultra indicative: Aroon Up/Down shows how recently highs/lows occurred; consolidation detector.'},
 {id:'willr',group:'Ultra momentum & squeeze',name:'Williams %R · 14 — momentum extremes',mode:'panel',note:'Ultra indicative: Williams %R momentum oscillator; -20/-80 overbought/oversold.'},
 {id:'roc',group:'Ultra momentum & squeeze',name:'Rate of Change · 12 — velocity',mode:'panel',note:'Ultra indicative: ROC velocity; pure momentum with zero-center.'},
 {id:'stochRsi',group:'Ultra momentum & squeeze',name:'Stochastic RSI · 14,14,3,3 — RSI sensitivity',mode:'panel',note:'Ultra indicative: Stochastic RSI amplifies RSI extremes for early entries.'},
 {id:'bbWidth',group:'Ultra momentum & squeeze',name:'Bollinger Bandwidth & %B · 20,2σ — squeeze',mode:'panel',note:'Ultra indicative: Bandwidth squeezes precede expansion; %B shows position in bands.'},
 {id:'coppock',group:'Ultra cycle & accumulation',name:'Coppock Curve · 11,14,10 — long-term bottoms',mode:'panel',note:'Ultra indicative: Coppock long-term momentum bottom detector; institutional accumulation.'},
 {id:'elder',group:'Ultra cycle & accumulation',name:'Elder Impulse System · EMA13/MACD — impulse',mode:'panel',note:'Ultra indicative: Elder impulse colors bars by EMA slope + MACD histogram; green/red/blue.'}
];
const FEATURE_MAP = new Map(FEATURES.map(f=>[f.id,f]));
const PANEL_IDS = FEATURES.filter(f=>f.mode==='panel').map(f=>f.id);
let start = Math.max(0,N-200), end = N;
let hoverIndex = -1, dpr = 1, layout = null, viewMode = 'candles';
let anchorIndex = null, armAnchor = false, ruler = null, dragState = null;
let profileCache = null, profileCacheKey = '', featureQuery = '';

function mean(values){if(!values.length)return null;return values.reduce((a,b)=>a+b,0)/values.length;}
function sma(a,n){const out=Array(a.length).fill(null);let sum=0;for(let i=0;i<a.length;i++){sum+=a[i];if(i>=n)sum-=a[i-n];if(i>=n-1)out[i]=sum/n;}return out;}
function rollingStd(a,n){const out=Array(a.length).fill(null);let sum=0,sq=0;for(let i=0;i<a.length;i++){sum+=a[i];sq+=a[i]*a[i];if(i>=n){sum-=a[i-n];sq-=a[i-n]*a[i-n];}if(i>=n-1){const m=sum/n;out[i]=Math.sqrt(Math.max(0,sq/n-m*m));}}return out;}
function rollingHigh(a,n){const out=Array(a.length).fill(null);for(let i=n-1;i<a.length;i++){let v=-Infinity;for(let j=i-n+1;j<=i;j++)v=Math.max(v,a[j]);out[i]=v;}return out;}
function rollingLow(a,n){const out=Array(a.length).fill(null);for(let i=n-1;i<a.length;i++){let v=Infinity;for(let j=i-n+1;j<=i;j++)v=Math.min(v,a[j]);out[i]=v;}return out;}
function ema(a,n){const out=Array(a.length).fill(null);if(a.length<n)return out;let seed=0;for(let i=0;i<n;i++)seed+=a[i];let prev=seed/n;out[n-1]=prev;const k=2/(n+1);for(let i=n;i<a.length;i++){prev=a[i]*k+prev*(1-k);out[i]=prev;}return out;}
function rma(a,n){const out=Array(a.length).fill(null);if(a.length<n)return out;let seed=0;for(let i=0;i<n;i++)seed+=a[i];let prev=seed/n;out[n-1]=prev;for(let i=n;i<a.length;i++){prev=(prev*(n-1)+a[i])/n;out[i]=prev;}return out;}
function rollingSum(a,n){const out=Array(a.length).fill(null);let s=0;for(let i=0;i<a.length;i++){s+=a[i];if(i>=n)s-=a[i-n];if(i>=n-1)out[i]=s;}return out;}
function safeDiv(a,b){return Math.abs(b)<1e-18?null:a/b;}
function last(a){for(let i=a.length-1;i>=0;i--)if(Number.isFinite(a[i]))return a[i];return null;}
const O=C.map(x=>x.open), H=C.map(x=>x.high), L=C.map(x=>x.low), X=C.map(x=>x.close), V=C.map(x=>Math.max(0,x.volume));
const T=C.map((x,i)=>(x.high+x.low+x.close)/3);
const TR=C.map((x,i)=>i===0?x.high-x.low:Math.max(x.high-x.low,Math.abs(x.high-C[i-1].close),Math.abs(x.low-C[i-1].close)));
const ATR14=rma(TR,14), ATR10=rma(TR,10), ATR20=rma(TR,20);
const pivots=[];
for(let i=3;i<N-3;i++){
  let isHigh=true,isLow=true;
  for(let j=i-3;j<=i+3;j++){if(j===i)continue;if(H[j]>H[i]||(H[j]===H[i]&&j<i))isHigh=false;if(L[j]<L[i]||(L[j]===L[i]&&j<i))isLow=false;}
  if(isHigh)pivots.push({i,price:H[i],kind:'high'});
  if(isLow)pivots.push({i,price:L[i],kind:'low'});
}
pivots.sort((a,b)=>a.i-b.i||a.kind.localeCompare(b.kind));
const pivotHighs=pivots.filter(p=>p.kind==='high'), pivotLows=pivots.filter(p=>p.kind==='low');
const pivotHighByIndex=new Map(pivotHighs.map(p=>[p.i,p]));
const pivotLowByIndex=new Map(pivotLows.map(p=>[p.i,p]));
const structureEvents=[], equalEvents=[], sweepEvents=[], gapEvents=[], fvgZones=[], orderBlocks=[], breakerEvents=[];
let lastPH=null,lastPL=null,lastBreakDir=0,lastPHBroken=-1,lastPLBroken=-1,priorPHForEqual=null,priorPLForEqual=null;
const usedSweeps=new Set();
for(let i=0;i<N;i++){
  if(pivotHighByIndex.has(i)){
    const p=pivotHighByIndex.get(i);
    if(priorPHForEqual){const tol=(ATR14[i]||p.price*.001)*.15;if(Math.abs(priorPHForEqual.price-p.price)<=tol)equalEvents.push({i,price:(priorPHForEqual.price+p.price)/2,kind:'high'});}
    priorPHForEqual=p;lastPH=p;
  }
  if(pivotLowByIndex.has(i)){
    const p=pivotLowByIndex.get(i);
    if(priorPLForEqual){const tol=(ATR14[i]||p.price*.001)*.15;if(Math.abs(priorPLForEqual.price-p.price)<=tol)equalEvents.push({i,price:(priorPLForEqual.price+p.price)/2,kind:'low'});}
    priorPLForEqual=p;lastPL=p;
  }
  if(i>0&&O[i]>H[i-1])gapEvents.push({i,top:O[i],bottom:H[i-1],kind:'up'});
  else if(i>0&&O[i]<L[i-1])gapEvents.push({i,top:L[i-1],bottom:O[i],kind:'down'});
  if(i>=2&&L[i]>H[i-2])fvgZones.push({start:i-2,end:N-1,top:L[i],bottom:H[i-2],kind:'up',i});
  else if(i>=2&&H[i]<L[i-2])fvgZones.push({start:i-2,end:N-1,top:L[i-2],bottom:H[i],kind:'down',i});
  if(lastPH&&i>lastPH.i&&i>lastPHBroken&&X[i]>lastPH.price){
    const kind=lastBreakDir===-1?'choch-up':'bos-up';structureEvents.push({i,price:X[i],kind});lastBreakDir=1;lastPHBroken=i;
    const from=Math.max(0,i-12);let k=i-1;while(k>=from&&C[k].close>=C[k].open)k--;
    if(k>=from)orderBlocks.push({start:k,end:Math.min(N-1,i+30),top:H[k],bottom:L[k],kind:'up',i});
  }else if(lastPL&&i>lastPL.i&&i>lastPLBroken&&X[i]<lastPL.price){
    const kind=lastBreakDir===1?'choch-down':'bos-down';structureEvents.push({i,price:X[i],kind});lastBreakDir=-1;lastPLBroken=i;
    const from=Math.max(0,i-12);let k=i-1;while(k>=from&&C[k].close<=C[k].open)k--;
    if(k>=from)orderBlocks.push({start:k,end:Math.min(N-1,i+30),top:H[k],bottom:L[k],kind:'down',i});
  }
  if(lastPH&&i>lastPH.i&&!usedSweeps.has(lastPH.i)&&H[i]>lastPH.price&&X[i]<lastPH.price){sweepEvents.push({i,price:H[i],kind:'high'});usedSweeps.add(lastPH.i);}
  if(lastPL&&i>lastPL.i&&!usedSweeps.has(-lastPL.i-1)&&L[i]<lastPL.price&&X[i]>lastPL.price){sweepEvents.push({i,price:L[i],kind:'low'});usedSweeps.add(-lastPL.i-1);}
}
if(N){let treeSize=1;while(treeSize<N)treeSize<<=1;const minTree=Array(treeSize*2).fill(Infinity),maxTree=Array(treeSize*2).fill(-Infinity),minCloseTree=Array(treeSize*2).fill(Infinity),maxCloseTree=Array(treeSize*2).fill(-Infinity);for(let i=0;i<N;i++){minTree[treeSize+i]=L[i];maxTree[treeSize+i]=H[i];minCloseTree[treeSize+i]=X[i];maxCloseTree[treeSize+i]=X[i];}for(let i=treeSize-1;i>0;i--){minTree[i]=Math.min(minTree[i*2],minTree[i*2+1]);maxTree[i]=Math.max(maxTree[i*2],maxTree[i*2+1]);minCloseTree[i]=Math.min(minCloseTree[i*2],minCloseTree[i*2+1]);maxCloseTree[i]=Math.max(maxCloseTree[i*2],maxCloseTree[i*2+1]);}
 function firstAtMost(node,left,right,from,target){if(right<from||minTree[node]>target)return-1;if(left===right)return left<N?left:-1;const mid=(left+right)>>1;const a=firstAtMost(node*2,left,mid,from,target);return a>=0?a:firstAtMost(node*2+1,mid+1,right,from,target);}
 function firstAtLeast(node,left,right,from,target){if(right<from||maxTree[node]<target)return-1;if(left===right)return left<N?left:-1;const mid=(left+right)>>1;const a=firstAtLeast(node*2,left,mid,from,target);return a>=0?a:firstAtLeast(node*2+1,mid+1,right,from,target);}
 function firstCloseBelow(node,left,right,from,target){if(right<from||minCloseTree[node]>=target)return-1;if(left===right)return left<N?left:-1;const mid=(left+right)>>1;const a=firstCloseBelow(node*2,left,mid,from,target);return a>=0?a:firstCloseBelow(node*2+1,mid+1,right,from,target);}
 function firstCloseAbove(node,left,right,from,target){if(right<from||maxCloseTree[node]<=target)return-1;if(left===right)return left<N?left:-1;const mid=(left+right)>>1;const a=firstCloseAbove(node*2,left,mid,from,target);return a>=0?a:firstCloseAbove(node*2+1,mid+1,right,from,target);}
 for(const z of fvgZones){const fill=z.kind==='up'?firstAtMost(1,0,treeSize-1,z.i+1,z.top):firstAtLeast(1,0,treeSize-1,z.i+1,z.bottom);if(fill>=0)z.end=fill;}
 for(const z of orderBlocks){const broken=z.kind==='up'?firstCloseBelow(1,0,treeSize-1,z.i+1,z.bottom):firstCloseAbove(1,0,treeSize-1,z.i+1,z.top);if(broken>=0)breakerEvents.push({i:broken,price:X[broken],kind:z.kind});}
}
const closeSMA20=sma(X,20), closeEMA20=ema(X,20), closeEMA50=ema(X,50), volSMA20=sma(V,20), closeSTD20=rollingStd(X,20);
const bbUpper=closeSMA20.map((v,i)=>v==null?null:v+2*(closeSTD20[i]||0));
const bbLower=closeSMA20.map((v,i)=>v==null?null:v-2*(closeSTD20[i]||0));
const vwap=Array(N).fill(null);let dayKey='',pv=0,pvol=0;
for(let i=0;i<N;i++){const d=new Date(C[i].time);const key=Number.isFinite(d.getTime())?d.toISOString().slice(0,10):'unknown';if(key!==dayKey){dayKey=key;pv=0;pvol=0;}pv+=T[i]*V[i];pvol+=V[i];vwap[i]=pvol?pv/pvol:T[i];}
function anchoredVwap(anchor){const out=Array(N).fill(null);let a=0,b=0;for(let i=Math.max(0,anchor);i<N;i++){a+=T[i]*V[i];b+=V[i];out[i]=b?a/b:T[i];}return out;}
const vwma20=(()=>{const n=20,num=sma(X.map((x,i)=>x*V[i]),n),den=sma(V,n);return num.map((x,i)=>x==null||!den[i]?null:x/den[i]);})();
const donchHi=rollingHigh(H,20),donchLo=rollingLow(L,20),tenkan=Array(N).fill(null),kijun=Array(N).fill(null);
function rollingMid(i,n){if(i<n-1)return null;let hi=-Infinity,lo=Infinity;for(let j=i-n+1;j<=i;j++){hi=Math.max(hi,H[j]);lo=Math.min(lo,L[j]);}return(hi+lo)/2;}
for(let i=0;i<N;i++){tenkan[i]=rollingMid(i,9);kijun[i]=rollingMid(i,26);}
const keltnerUpper=closeEMA20.map((x,i)=>x==null||ATR20[i]==null?null:x+1.5*ATR20[i]);
const keltnerLower=closeEMA20.map((x,i)=>x==null||ATR20[i]==null?null:x-1.5*ATR20[i]);
const supertrend=Array(N).fill(null),superDir=Array(N).fill(0);let fu=null,fl=null,dir=1;
for(let i=0;i<N;i++){
 if(ATR10[i]==null)continue;const mid=(H[i]+L[i])/2,bu=mid+3*ATR10[i],bl=mid-3*ATR10[i];
 if(fu==null){fu=bu;fl=bl;dir=X[i]>=mid?1:-1;}else{const oldU=fu,oldL=fl;fu=(bu<oldU||X[i-1]>oldU)?bu:oldU;fl=(bl>oldL||X[i-1]<oldL)?bl:oldL;if(dir<0&&X[i]>fu)dir=1;else if(dir>0&&X[i]<fl)dir=-1;}
 superDir[i]=dir;supertrend[i]=dir>0?fl:fu;
}
const delta=V.map((v,i)=>v*(C[i].close>C[i].open?1:C[i].close<C[i].open?-1:0));
const cvd=Array(N).fill(0),obv=Array(N).fill(0),adl=Array(N).fill(0),vpt=Array(N).fill(0);
for(let i=0;i<N;i++){
 const mfm=H[i]===L[i]?0:((2*X[i]-H[i]-L[i])/(H[i]-L[i]));
 cvd[i]=(i?cvd[i-1]:0)+delta[i];obv[i]=(i?obv[i-1]:0)+(i&&X[i]<X[i-1]?-V[i]:i&&X[i]>X[i-1]?V[i]:0);
 adl[i]=(i?adl[i-1]:0)+mfm*V[i];vpt[i]=(i?vpt[i-1]:0)+(i&&X[i-1]?((X[i]-X[i-1])/X[i-1])*V[i]:0);
}
const cmf=Array(N).fill(null),mfv=H.map((h,i)=>(h===L[i]?0:(2*X[i]-h-L[i])/(h-L[i]))*V[i]),mfvSum=rollingSum(mfv,20),volSum20=rollingSum(V,20);
for(let i=0;i<N;i++)cmf[i]=mfvSum[i]==null||!volSum20[i]?null:mfvSum[i]/volSum20[i];
const mfi=Array(N).fill(null);let posMF=Array(N).fill(0),negMF=Array(N).fill(0);
for(let i=1;i<N;i++){const f=T[i]*V[i];if(T[i]>T[i-1])posMF[i]=f;else if(T[i]<T[i-1])negMF[i]=f;}
const posSum=rollingSum(posMF,14),negSum=rollingSum(negMF,14);
for(let i=0;i<N;i++)mfi[i]=posSum[i]==null?null:negSum[i]===0?100:100-100/(1+posSum[i]/negSum[i]);
const force=ema(X.map((x,i)=>i?(x-X[i-1])*V[i]:0),13);
const impact=Array(N).fill(null);for(let i=1;i<N;i++){const notional=Math.max(1e-12,V[i]*Math.max(Math.abs(X[i]),1e-12)/1e6);impact[i]=Math.abs(Math.log(Math.max(X[i],1e-12)/Math.max(X[i-1],1e-12)))/notional*10000;}
const rsi=(()=>{const up=Array(N).fill(0),dn=Array(N).fill(0);for(let i=1;i<N;i++){const d=X[i]-X[i-1];up[i]=Math.max(0,d);dn[i]=Math.max(0,-d);}const au=rma(up,14),ad=rma(dn,14);return au.map((x,i)=>x==null?null:ad[i]===0?100:100-100/(1+x/ad[i]));})();
const ema12=ema(X,12),ema26=ema(X,26),macd=ema12.map((x,i)=>x==null||ema26[i]==null?null:x-ema26[i]);
const macdSignal=(()=>{const first=macd.findIndex(x=>x!=null);const out=Array(N).fill(null);if(first<0)return out;const tail=ema(macd.slice(first).map(x=>x??0),9);for(let i=0;i<tail.length;i++)out[first+i]=tail[i];return out;})();
const macdHist=macd.map((x,i)=>x==null||macdSignal[i]==null?null:x-macdSignal[i]);
const stochRaw=Array(N).fill(null);for(let i=13;i<N;i++){let lo=Infinity,hi=-Infinity;for(let j=i-13;j<=i;j++){lo=Math.min(lo,L[j]);hi=Math.max(hi,H[j]);}stochRaw[i]=hi===lo?50:100*(X[i]-lo)/(hi-lo);}
const stochK=sma(stochRaw.map(x=>x??0),3).map((x,i)=>i<15?null:x),stochD=sma(stochK.map(x=>x??0),3).map((x,i)=>i<17?null:x);
const plusDM=Array(N).fill(0),minusDM=Array(N).fill(0);
for(let i=1;i<N;i++){const up=H[i]-H[i-1],dn=L[i-1]-L[i];if(up>dn&&up>0)plusDM[i]=up;if(dn>up&&dn>0)minusDM[i]=dn;}
const plusR=rma(plusDM,14),minusR=rma(minusDM,14),plusDI=Array(N).fill(null),minusDI=Array(N).fill(null),dx=Array(N).fill(0);
for(let i=0;i<N;i++){if(plusR[i]!=null&&ATR14[i]){plusDI[i]=100*plusR[i]/ATR14[i];minusDI[i]=100*minusR[i]/ATR14[i];dx[i]=100*Math.abs(plusDI[i]-minusDI[i])/Math.max(plusDI[i]+minusDI[i],1e-12);}}
const adx=rma(dx,14),cci=Array(N).fill(null),typMean=sma(T,20);
for(let i=19;i<N;i++){let dev=0;for(let j=i-19;j<=i;j++)dev+=Math.abs(T[j]-typMean[i]);const md=dev/20;cci[i]=md?((T[i]-typMean[i])/(.015*md)):0;}
const logReturns=Array(N).fill(0);for(let i=1;i<N;i++)logReturns[i]=Math.log(Math.max(X[i],1e-12)/Math.max(X[i-1],1e-12));
const realizedVol=rollingStd(logReturns,20),parkinson=Array(N).fill(null);
for(let i=19;i<N;i++){let s=0;for(let j=i-19;j<=i;j++)if(H[j]>0&&L[j]>0)s+=Math.log(H[j]/L[j])**2;parkinson[i]=Math.sqrt(s/(4*20*Math.log(2)));}
const drawdown=Array(N).fill(0);let peak=-Infinity;for(let i=0;i<N;i++){peak=Math.max(peak,X[i]);drawdown[i]=peak?X[i]/peak-1:0;}
const zscore=closeSMA20.map((x,i)=>x==null||!closeSTD20[i]?null:(X[i]-x)/closeSTD20[i]);
// --- 10 ultra indicative series (HMA, KAMA, PSAR, ARoon, WillR, ROC, StochRSI, BBWidth/%B, Coppock, Elder) ---
function wma(arr,n){const out=Array(arr.length).fill(null);let sum=0,denom=n*(n+1)/2;for(let i=0;i<arr.length;i++){if(i<n){sum+=arr[i]*(i+1);if(i===n-1)out[i]=sum/denom;continue;}let weighted=0;for(let j=0;j<n;j++)weighted+=arr[i-j]*(n-j);out[i]=weighted/denom;}return out;}
function wmaSimple(values,n){const out=Array(values.length).fill(null);for(let i=n-1;i<values.length;i++){let w=0,s=0;for(let j=0;j<n;j++){const weight=n-j;w+=values[i-j]*weight;s+=weight;}out[i]=w/s;}return out;}
const hma21=(()=>{const half=Math.floor(21/2), wmaHalf=wmaSimple(X,half), wmaFull=wmaSimple(X,21);const raw=wmaHalf.map((v,i)=>v==null||wmaFull[i]==null?null:2*v-wmaFull[i]);return wmaSimple(raw.map(v=>v??0),Math.round(Math.sqrt(21))).map((v,i)=>raw[i]==null?null:v);})();
const kama=(()=>{const n=10, fast=2, slow=30;const out=Array(N).fill(null);let prev=null;for(let i=0;i<N;i++){if(i<n){out[i]=null;continue;}let dir=Math.abs(X[i]-X[i-n]);let vol=0;for(let k=i-n+1;k<=i;k++)vol+=Math.abs(X[k]-X[k-1]);const er=vol===0?0:dir/vol;const sc=Math.pow(er*(2/(fast+1)-2/(slow+1))+2/(slow+1),2);prev=prev==null?X[i]:prev+sc*(X[i]-prev);out[i]=prev;}return out;})();
const psar=(()=>{const out=Array(N).fill(null);if(N<2)return out;let isLong=true;let af=0.02, ep=H[0], sar=L[0];for(let i=1;i<N;i++){if(i===1){out[i]=sar;continue;}if(isLong){if(L[i]<sar){isLong=false;sar=ep;ep=L[i];af=0.02;out[i]=sar;continue;}if(H[i]>ep){ep=H[i];af=Math.min(0.20,af+0.02);}sar=sar+af*(ep-sar);if(sar>H[i-1]||sar>H[i-2])sar=Math.min(H[i-1],H[i-2]);out[i]=sar;}else{if(H[i]>sar){isLong=true;sar=ep;ep=H[i];af=0.02;out[i]=sar;continue;}if(L[i]<ep){ep=L[i];af=Math.min(0.20,af+0.02);}sar=sar+af*(ep-sar);if(sar<L[i-1]||sar<L[i-2])sar=Math.max(L[i-1],L[i-2]);out[i]=sar;}}return out;})();
const aroonUp=Array(N).fill(null), aroonDown=Array(N).fill(null);for(let i=14;i<N;i++){let hhIdx=i, llIdx=i, hh=-Infinity, ll=Infinity;for(let j=i-14;j<=i;j++){if(H[j]>=hh){hh=H[j];hhIdx=j;}if(L[j]<=ll){ll=L[j];llIdx=j;}}aroonUp[i]=(14-(i-hhIdx))/14*100;aroonDown[i]=(14-(i-llIdx))/14*100;}
const willr=Array(N).fill(null);for(let i=13;i<N;i++){let hh=-Infinity, ll=Infinity;for(let j=i-13;j<=i;j++){hh=Math.max(hh,H[j]);ll=Math.min(ll,L[j]);}willr[i]=hh===ll?-50:(hh-X[i])/(hh-ll)*-100;}
const roc12=Array(N).fill(null);for(let i=12;i<N;i++){const prev=X[i-12];roc12[i]=prev===0?0:(X[i]-prev)/Math.abs(prev)*100;}
const stochRsiK=Array(N).fill(null), stochRsiD=Array(N).fill(null);(()=>{const rsiWindow=Array(N).fill(null);const rsiVals=rsi;for(let i=0;i<N;i++){rsiWindow[i]=rsiVals[i];}for(let i=13;i<N;i++){let mn=Infinity,mx=-Infinity;for(let j=i-13;j<=i;j++){const v=rsiWindow[j];if(v==null)continue;mn=Math.min(mn,v);mx=Math.max(mx,v);}if(!Number.isFinite(mn)||!Number.isFinite(mx)||mx===mn){stochRsiK[i]=50;continue;}stochRsiK[i]=(rsiVals[i]-mn)/(mx-mn)*100;}const kSmooth=sma(stochRsiK.map(v=>v??50),3);for(let i=0;i<N;i++)if(kSmooth[i]!=null)stochRsiK[i]=kSmooth[i];const dSmooth=sma(stochRsiK.map(v=>v??50),3);for(let i=0;i<N;i++)if(dSmooth[i]!=null)stochRsiD[i]=dSmooth[i];})();
const bbBandwidth=Array(N).fill(null), bbPercentB=Array(N).fill(null);for(let i=0;i<N;i++){if(bbUpper[i]==null||bbLower[i]==null||closeSMA20[i]==null)continue;const bw=(bbUpper[i]-bbLower[i])/(closeSMA20[i]||1)*100;bbBandwidth[i]=bw;const range=bbUpper[i]-bbLower[i];bbPercentB[i]=range===0?0.5:(X[i]-bbLower[i])/range*100;}
const coppock=(()=>{const roc11=Array(N).fill(null), roc14b=Array(N).fill(null);for(let i=11;i<N;i++)roc11[i]=X[i-11]===0?0:(X[i]-X[i-11])/Math.abs(X[i-11])*100;for(let i=14;i<N;i++)roc14b[i]=X[i-14]===0?0:(X[i]-X[i-14])/Math.abs(X[i-14])*100;const sum=roc11.map((v,i)=>v==null||roc14b[i]==null?null:v+roc14b[i]);return wmaSimple(sum.map(v=>v??0),10).map((v,i)=>sum[i]==null?null:v);})();
const elderImpulse=Array(N).fill(null), elderColor=Array(N).fill(0);for(let i=0;i<N;i++){if(closeEMA20[i]==null||i===0||closeEMA20[i-1]==null||macdHist[i]==null||macdHist[i-1]==null)continue;const emaUp=closeEMA20[i]>closeEMA20[i-1], emaDown=closeEMA20[i]<closeEMA20[i-1];const histUp=macdHist[i]>macdHist[i-1], histDown=macdHist[i]<macdHist[i-1];if(emaUp&&histUp)elderColor[i]=1;else if(emaDown&&histDown)elderColor[i]=-1;else elderColor[i]=0;elderImpulse[i]=elderColor[i]*10;}
const HMA21=hma21; const KAMA=kama;
const atrLong=X.map((x,i)=>ATR14[i]==null?null:x-2*ATR14[i]),atrShort=X.map((x,i)=>ATR14[i]==null?null:x+2*ATR14[i]);
const SERIES={sma20:closeSMA20,ema20:closeEMA20,ema50:closeEMA50,vwap,avwap:null,bbUpper,bbLower,keltnerUpper,keltnerLower,donchHi,donchLo,supertrend,tenkan,kijun,vwma20,atrLong,atrShort,hma21:HMA21,kama:KAMA,psar};
let avwapSeries=anchoredVwap(Math.max(0,N-1));
function chooseDefaultAnchor(){const lastLow=pivotLows.at(-1);return lastLow?lastLow.i:Math.max(0,N-50);}
anchorIndex=chooseDefaultAnchor();avwapSeries=anchoredVwap(anchorIndex);SERIES.avwap=avwapSeries;
const PANELS={
 delta:{name:'Candle-direction volume delta · proxy',a:delta,zero:true,fmt:v=>formatVolume(v)},
 cvd:{name:'Cumulative candle-volume delta · proxy',a:cvd,zero:true,fmt:v=>formatVolume(v)},
 obv:{name:'On-balance volume',a:obv,zero:true,fmt:v=>formatVolume(v)},
 cmf:{name:'Chaikin money flow · 20',a:cmf,range:[-1,1],zero:true,fmt:v=>v.toFixed(3)},
 mfi:{name:'Money Flow Index · 14',a:mfi,range:[0,100],guides:[30,70],fmt:v=>v.toFixed(1)},
 adl:{name:'Accumulation / distribution line',a:adl,zero:true,fmt:v=>formatVolume(v)},
 force:{name:'Force Index · EMA 13',a:force,zero:true,fmt:v=>formatVolume(v)},
 vpt:{name:'Volume-price trend',a:vpt,zero:true,fmt:v=>formatVolume(v)},
 rvol:{name:'Relative volume · 20',a:V.map((v,i)=>volSMA20[i]?v/volSMA20[i]:null),guides:[1,2],fmt:v=>v.toFixed(2)+'×'},
 amihud:{name:'Amihud price-impact proxy',a:impact,zero:false,fmt:v=>v.toFixed(2)+' bp / $1m'},
 rsi:{name:'Relative Strength Index · 14',a:rsi,range:[0,100],guides:[30,70],fmt:v=>v.toFixed(1)},
 macd:{name:'MACD · 12, 26, 9',a:macd,b:macdSignal,hist:macdHist,zero:true,fmt:v=>formatPrice(v)},
 stochastic:{name:'Stochastic · 14, 3',a:stochK,b:stochD,range:[0,100],guides:[20,80],fmt:v=>v.toFixed(1)},
 adx:{name:'ADX / +DI / −DI · 14',a:adx,b:plusDI,c:minusDI,range:[0,100],guides:[25],fmt:v=>v.toFixed(1)},
 atr:{name:'Average True Range · 14',a:ATR14,zero:false,fmt:v=>formatPrice(v)},
 cci:{name:'Commodity Channel Index · 20',a:cci,zero:true,guides:[-100,100],fmt:v=>v.toFixed(1)},
 'realized-vol':{name:'20-bar realized volatility',a:realizedVol,zero:false,fmt:v=>(v*100).toFixed(3)+'%'},
 parkinson:{name:'Parkinson range volatility · 20',a:parkinson,zero:false,fmt:v=>(v*100).toFixed(3)+'%'},
 drawdown:{name:'Running-peak drawdown',a:drawdown,range:[null,0],zero:true,fmt:v=>(v*100).toFixed(2)+'%'},
 zscore:{name:'Close z-score · 20',a:zscore,zero:true,guides:[-2,2],fmt:v=>v.toFixed(2)},
 aroon:{name:'Aroon Up/Down · 14 — ultra trend',a:aroonUp,b:aroonDown,range:[0,100],guides:[30,70],fmt:v=>v.toFixed(1)},
 willr:{name:'Williams %R · 14 — ultra momentum',a:willr,range:[-100,0],guides:[-20,-80],fmt:v=>v.toFixed(1)},
 roc:{name:'Rate of Change · 12 — ultra velocity',a:roc12,zero:true,fmt:v=>v.toFixed(2)+'%' },
 stochRsi:{name:'Stochastic RSI · 14,14,3,3 — ultra sensitivity',a:stochRsiK,b:stochRsiD,range:[0,100],guides:[20,80],fmt:v=>v.toFixed(1)},
 bbWidth:{name:'Bollinger Bandwidth & %B · 20 — ultra squeeze',a:bbBandwidth,b:bbPercentB,zero:false,fmt:v=>v.toFixed(2)+'%' },
 coppock:{name:'Coppock Curve · 11,14,10 — ultra bottom',a:coppock,zero:true,fmt:v=>v.toFixed(2)},
 elder:{name:'Elder Impulse System — ultra impulse',a:elderImpulse,zero:true,fmt:v=>v>5?'bullish':v<-5?'bearish':'neutral'}
};
function formatPrice(v){if(v==null||!Number.isFinite(v))return'—';const digits=Math.abs(v)<1?6:Math.abs(v)<100?4:2;return Number(v).toLocaleString(undefined,{maximumFractionDigits:digits});}
function formatVolume(v){if(v==null||!Number.isFinite(v))return'—';const a=Math.abs(v);if(a>=1e9)return(v/1e9).toFixed(2)+'b';if(a>=1e6)return(v/1e6).toFixed(2)+'m';if(a>=1e3)return(v/1e3).toFixed(2)+'k';return v.toFixed(2);}
function formatPct(v){return v==null||!Number.isFinite(v)?'—':(v>=0?'+':'')+(v*100).toFixed(2)+'%';}
function escapeHtml(s){return String(s).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));}
function timeText(i,withTime=true){if(!C[i]||!Number.isFinite(C[i].time))return`bar ${i+1}`;const d=new Date(C[i].time);return withTime?d.toLocaleString(undefined,{timeZone:'UTC',month:'short',day:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit',hour12:false})+' UTC':d.toLocaleDateString(undefined,{timeZone:'UTC',month:'short',day:'2-digit',year:'2-digit'});}
function axisTimeText(i,span){if(!C[i]||!Number.isFinite(C[i].time))return`#${i+1}`;const d=new Date(C[i].time),date=d.toLocaleDateString(undefined,{timeZone:'UTC',month:'short',day:'2-digit'});return span<180?`${date} ${d.toLocaleTimeString(undefined,{timeZone:'UTC',hour:'2-digit',minute:'2-digit',hour12:false})}`:date;}
function sessionCode(i){const h=new Date(C[i].time).getUTCHours();if(h>=0&&h<8)return 0;if(h>=7&&h<16)return 1;if(h>=13&&h<22)return 2;return -1;}
function getPanelId(){return PANEL_IDS.find(id=>active.has(id))||null;}
function regimeText(){if(!N||!last(adx)||!last(realizedVol))return'Insufficient history';const vals=realizedVol.slice(-100).filter(Number.isFinite).sort((a,b)=>a-b);const med=vals.length?vals[Math.floor(vals.length/2)]:0;const rv=last(realizedVol);if(med&&rv>med*1.7)return'High volatility';return last(adx)>=25?'Directional':'Range / rotation';}
function excursionText(){const p=pivots.at(-1);if(!p||p.i>=N-1)return'Need a completed pivot-to-current leg.';let hi=-Infinity,lo=Infinity;for(let i=p.i;i<N;i++){hi=Math.max(hi,H[i]);lo=Math.min(lo,L[i]);}if(p.kind==='low')return`From pivot low: MFE ${formatPct((hi-p.price)/p.price)} · MAE ${formatPct((lo-p.price)/p.price)}`;return`From pivot high: favorable ${formatPct((p.price-lo)/p.price)} · adverse ${formatPct((p.price-hi)/p.price)}`;}
function updateStats(){
 if(!N)return;const lastC=C[N-1],first=C[Math.max(0,start)];
 document.getElementById('statPrice').textContent=formatPrice(lastC.close);
 const change=first&&first.close?lastC.close/first.close-1:0;const changeEl=document.getElementById('statChange');changeEl.textContent=formatPct(change);changeEl.className=change>=0?'positive':'negative';
 const rv=PANELS.rvol.a[N-1];document.getElementById('statRvol').textContent=rv==null?'—':rv.toFixed(2)+'×';
 const vw=vwap[N-1];const dist=vw?lastC.close/vw-1:null;const vwEl=document.getElementById('statVwap');vwEl.textContent=formatPct(dist);vwEl.className=dist==null?'':dist>=0?'positive':'negative';
 const d20=delta.slice(Math.max(0,N-20)).reduce((a,b)=>a+b,0);const de=document.getElementById('statDelta');de.textContent=formatVolume(d20)+(d20>0?' up-candle proxy':d20<0?' down-candle proxy':' flat');de.className=d20>=0?'positive':'negative';
 document.getElementById('statPatterns').textContent=String(PATTERNS.length);
 const badge=document.getElementById('regimeBadge');badge.textContent=active.has('regime')?`Regime: ${regimeText()}`:'';
 document.getElementById('paneBadge').textContent=getPanelId()?`Pane: ${FEATURE_MAP.get(getPanelId()).name}`:'';
 const anchor=document.getElementById('anchorButton');anchor.classList.toggle('selected',armAnchor);
 const count=document.getElementById('featureCount');count.textContent=`${active.size} / 60 active`;
 const card=document.getElementById('sizeCard');card.classList.toggle('visible',active.has('sizing'));
 const ex=active.has('excursions'),foot=document.getElementById('footStatus');if(active.has('ruler'))foot.textContent=!ruler?'R ruler: drag entry to stop, then click a target price.':ruler.stage==='stop'?'Drag entry to a stop level, then release.':ruler.stage==='target'?'Risk leg set: click a target price to calculate R.':'R ruler measured; drag again to start over.';else if(ex)foot.textContent=excursionText();else if(!armAnchor)foot.textContent='Wheel to zoom · drag to pan · hover for OHLCV · double-click to fit';
 updateSizeCalculator();
}
function renderFeatureList(){
 const host=document.getElementById('featureList');const q=featureQuery.trim().toLowerCase();const filtered=FEATURES.filter(f=>!q||(f.name+' '+f.group+' '+f.note).toLowerCase().includes(q));
 const groups=[];for(const f of filtered){let g=groups.find(x=>x.name===f.group);if(!g){g={name:f.group,features:[]};groups.push(g);}g.features.push(f);}
 host.innerHTML=groups.map(g=>`<div class="group-title">${escapeHtml(g.name)}</div>`+g.features.map(f=>`<label class="feature" title="${escapeHtml(f.note)}"><input type="checkbox" data-feature="${f.id}" ${active.has(f.id)?'checked':''}><span class="feature-copy"><span class="feature-name">${escapeHtml(f.name)}</span><span class="feature-note">${escapeHtml(f.note)}</span></span></label>`).join('')).join('')||'<div class="feature-empty">No matching studies.</div>';
 host.querySelectorAll('[data-feature]').forEach(input=>input.addEventListener('change',()=>toggleFeature(input.dataset.feature,input.checked)));
 document.getElementById('featureCount').textContent=`${active.size} / 60 active`;
 renderLegend();
}
function renderLegend(){
 const names={patterns:'Patterns','swing-pivots':'Swings',structure:'BOS / CHoCH','equal-levels':'Equal levels',sweeps:'Sweeps',fvg:'FVG','order-blocks':'Order blocks','breaker-blocks':'Breakers',gaps:'Gaps',fib:'Fib','sma20':'SMA 20','ema20':'EMA 20','ema50':'EMA 50',vwap:'VWAP',avwap:'AVWAP',bollinger:'Bollinger',keltner:'Keltner',donchian:'Donchian',supertrend:'Supertrend',ichimoku:'Ichimoku',delta:'Delta proxy',cvd:'CVD proxy',obv:'OBV',cmf:'CMF',mfi:'MFI',adl:'A/D',force:'Force',vpt:'VPT',rvol:'RVOL','volume-profile':'Volume profile','profile-value':'POC / VA',vwma:'VWMA',amihud:'Price impact',rsi:'RSI',macd:'MACD',stochastic:'Stochastic',adx:'ADX',atr:'ATR',cci:'CCI','realized-vol':'Realized vol',parkinson:'Parkinson vol',drawdown:'Drawdown',zscore:'Z-score','atr-stops':'ATR rails',support:'Support / resistance',hma21:'HMA 21',kama:'KAMA',psar:'Parabolic SAR',aroon:'Aroon',willr:'Williams %R',roc:'ROC',stochRsi:'Stoch RSI',bbWidth:'BB Bandwidth',coppock:'Coppock',elder:'Elder Impulse'};
 const groupColors={'Structure & price action':'#f4bd62','Price overlays':'#76a8ff','Volume & money-flow proxies':'#48c8d8','Momentum & volatility':'#b49cff','Risk & context':'#42d6a0','Ultra trend & adaptive':'#ff9e66','Ultra momentum & squeeze':'#ff6b9e','Ultra cycle & accumulation':'#7ee787'};
 const viewNames={candles:'Candles',hollow:'Hollow',heikin:'Heikin–Ashi',line:'Close line'};
 const selected=FEATURES.filter(f=>active.has(f.id)&&!['ruler','sizing','excursions','regime','sessions'].includes(f.id));
 const limit=window.innerWidth<600?3:7; // 60 ultra studies, show more chips in full-client mode
 const chips=[{name:viewNames[viewMode]||'Candles',color:'#dce7ec'},{name:'Volume',color:'#42d6a0'},...selected.slice(0,limit).map(f=>({name:names[f.id]||f.name,color:groupColors[f.group]||'#82979d'}))];
 const more=selected.length>limit?`<span class="legend-more">+${selected.length-limit} studies</span>`:'';
 document.getElementById('chartLegend').innerHTML=chips.map(x=>`<span class="legend-chip"><i class="legend-dot" style="--legend-color:${x.color}"></i>${escapeHtml(x.name)}</span>`).join('')+more;
}
function toggleFeature(id,on){

 const f=FEATURE_MAP.get(id);if(!f)return;
 if(on){if(f.mode==='panel')for(const panel of PANEL_IDS)if(panel!==id)active.delete(panel);active.add(id);if(id==='profile-value')active.add('volume-profile');}else{active.delete(id);if(id==='volume-profile')active.delete('profile-value');}
 if(id==='ruler'){ruler=null;document.getElementById('footStatus').textContent=on?'R ruler: drag entry to stop, then click the target price.':'Wheel to zoom · drag to pan · hover for OHLCV · double-click to fit';}
 renderFeatureList();updateStats();draw();
}
function renderPatternLedger(){
 const byType=new Map();for(const p of PATTERNS)byType.set(p.type,(byType.get(p.type)||0)+1);
 document.getElementById('ledgerTotal').textContent=`${PATTERNS.length} matches`;
 const types=[...byType.entries()].sort((a,b)=>b[1]-a[1]);
 document.getElementById('patternTypes').innerHTML=types.slice(0,30).map(([name,count])=>`<span class="pattern-type" title="${escapeHtml(name)}">${escapeHtml(name)}<b>${count}</b></span>`).join('')+(types.length>30?`<span class="pattern-type">+${types.length-30} types</span>`:'');
 const recent=PATTERNS.map((p,idx)=>({...p,idx})).sort((a,b)=>b.end-a.end).slice(0,40);
 document.getElementById('patternRows').innerHTML=recent.map(p=>`<div class="pattern-row" data-pattern-index="${p.idx}" title="${escapeHtml(p.bias)} · bars ${p.start+1}–${p.end+1}"><span>${escapeHtml(p.type)} <i>${escapeHtml(p.bias)}</i></span><time>${escapeHtml(timeText(p.end,false))}</time></div>`).join('')||'<div class="side-sub">No pattern matches in this dataset.</div>';
 document.querySelectorAll('[data-pattern-index]').forEach(row=>row.addEventListener('click',()=>{const p=PATTERNS[+row.dataset.patternIndex];if(!p)return;active.add('patterns');renderFeatureList();const span=Math.min(Math.max(35,p.end-p.start+30),N);start=Math.max(0,Math.min(N-span,p.start-15));end=Math.min(N,start+span);draw();updateStats();}));
}
function updateSizeCalculator(){
 const card=document.getElementById('sizeCard');if(!card)return;
 const entry=document.getElementById('sizeEntry'),dist=document.getElementById('sizeDistance');
 if(!entry.value&&N)entry.value=String(C[N-1].close);
 if(!dist.value&&N)dist.value=String((last(ATR14)||Math.abs(C[N-1].close)*.01)*2);
 const equity=+document.getElementById('sizeEquity').value,risk=+document.getElementById('sizeRisk').value,price=+entry.value,distance=+dist.value;
 const out=document.getElementById('sizeResult');
 if(!(equity>0&&risk>0&&price>0&&distance>0)){out.textContent='Enter positive equity, risk, entry and stop distance.';return;}
 const budget=equity*risk/100,qty=budget/distance,notional=qty*price;
 out.innerHTML=`Risk budget <strong>${budget.toLocaleString(undefined,{maximumFractionDigits:2})}</strong><br>Approx. units <strong>${qty.toLocaleString(undefined,{maximumFractionDigits:8})}</strong> · notional ${notional.toLocaleString(undefined,{maximumFractionDigits:2})}<br><span style="color:#82979d">Before fees, slippage, leverage and contract rules.</span>`;
}
function calcVisibleProfile(a,b){
 const bins=48;let lo=Infinity,hi=-Infinity,total=0;
 for(let i=a;i<b;i++){lo=Math.min(lo,L[i]);hi=Math.max(hi,H[i]);total+=V[i];}
 if(!Number.isFinite(lo)||!Number.isFinite(hi))return null;
 if(hi===lo){hi=lo+Math.max(Math.abs(lo)*1e-8,1e-8);}
 const step=(hi-lo)/bins,vol=Array(bins).fill(0);
 for(let i=a;i<b;i++){
   const range=H[i]-L[i];
   if(range<=1e-16){const k=Math.max(0,Math.min(bins-1,Math.floor((T[i]-lo)/step)));vol[k]+=V[i];continue;}
   const first=Math.max(0,Math.floor((L[i]-lo)/step)),lastBin=Math.min(bins-1,Math.floor((H[i]-lo)/step));
   for(let k=first;k<=lastBin;k++){const bl=lo+k*step,bh=bl+step,overlap=Math.max(0,Math.min(H[i],bh)-Math.max(L[i],bl));if(overlap>0)vol[k]+=V[i]*overlap/range;}
 }
 let poc=0;for(let k=1;k<bins;k++)if(vol[k]>vol[poc])poc=k;
 const target=vol.reduce((x,y)=>x+y,0)*.70;let low=poc,high=poc,sum=vol[poc];
 while(sum<target&&(low>0||high<bins-1)){const down=low>0?vol[low-1]:-1,up=high<bins-1?vol[high+1]:-1;if(up>down){high++;sum+=Math.max(0,up);}else{low--;sum+=Math.max(0,down);}}
 return{lo,hi,step,vol,poc,low,high,max:Math.max(...vol),price:k=>lo+(k+.5)*step};
}
function haData(){const a=[];for(let i=0;i<N;i++){const close=(O[i]+H[i]+L[i]+X[i])/4,open=i?(a[i-1].open+a[i-1].close)/2:(O[i]+X[i])/2;a.push({open,close,high:Math.max(H[i],open,close),low:Math.min(L[i],open,close)});}return a;}
const HA=haData();
function activePriceArrays(){const a=[];for(const id of ['sma20','ema20','ema50','vwap','avwap','vwma','supertrend','atr-stops','bollinger','keltner','donchian','ichimoku','hma21','kama','psar'])if(active.has(id)){if(id==='vwma')a.push(vwma20);else if(id==='atr-stops')a.push(atrLong,atrShort);else if(id==='bollinger')a.push(bbUpper,bbLower);else if(id==='keltner')a.push(keltnerUpper,keltnerLower);else if(id==='donchian')a.push(donchHi,donchLo);else if(id==='ichimoku')a.push(tenkan,kijun);else if(id==='supertrend')a.push(supertrend);else if(id==='avwap')a.push(avwapSeries);else if(id==='hma21')a.push(HMA21);else if(id==='kama')a.push(KAMA);else if(id==='psar')a.push(psar);else a.push(SERIES[id]);}return a;}
function getWindow(){return{a:Math.max(0,Math.min(N-1,start)),b:Math.max(start+1,Math.min(N,end))};}
function fitAll(){if(!N)return;start=0;end=N;draw();updateStats();}
function setWindowCount(count){if(!N)return;const span=count==='all'?N:Math.min(N,+count);end=Math.max(1,end);start=Math.max(0,end-span);end=Math.min(N,start+span);draw();updateStats();}
function zoomAt(factor,anchor){if(!N)return;const old=end-start;const next=Math.max(8,Math.min(N,Math.round(old*factor)));const at=anchor??(start+old/2);const ratio=(at-start)/old;start=Math.round(at-ratio*next);end=start+next;if(start<0){end-=start;start=0;}if(end>N){start-=end-N;end=N;}start=Math.max(0,start);draw();updateStats();}
function currentViewCandle(i){return viewMode==='heikin'?HA[i]:C[i];}
function draw(){
 const rect=chart.getBoundingClientRect();if(!rect.width||!rect.height)return;
 dpr=Math.max(1,Math.min(4,window.devicePixelRatio||1));const pxW=Math.round(rect.width*dpr),pxH=Math.round(rect.height*dpr);
 if(chart.width!==pxW||chart.height!==pxH){chart.width=pxW;chart.height=pxH;}
 ctx.setTransform(dpr,0,0,dpr,0,0);const w=rect.width,h=rect.height,bg=ctx.createLinearGradient(0,0,0,h);bg.addColorStop(0,'#0b1820');bg.addColorStop(.58,'#081319');bg.addColorStop(1,'#071116');ctx.fillStyle=bg;ctx.fillRect(0,0,w,h);
 if(!N){document.getElementById('emptyMessage').style.display='flex';return;}document.getElementById('emptyMessage').style.display='none';
 start=Math.max(0,Math.min(start,N-1));end=Math.max(start+1,Math.min(end,N));const span=end-start;
 const profileOn=active.has('volume-profile');const left=68,right=w-(profileOn?125:20),top=78,axisH=25,volH=Math.min(78,h*.16),panelId=getPanelId(),panelH=panelId?Math.min(116,h*.22):0;
 const bottom=h-axisH-volH-panelH-17;const volTop=bottom+12,volBottom=volTop+volH,panelTop=volBottom+8,panelBottom=h-axisH-7;
 const plotW=Math.max(20,right-left),priceH=Math.max(80,bottom-top),candleW=plotW/span;
 let low=Infinity,high=-Infinity;
 const viewHigh=[],viewLow=[];
 for(let i=start;i<end;i++){const c=currentViewCandle(i);viewHigh.push(c.high);viewLow.push(c.low);low=Math.min(low,c.low);high=Math.max(high,c.high);}
 for(const arr of activePriceArrays())for(let i=start;i<end;i++)if(Number.isFinite(arr[i])){low=Math.min(low,arr[i]);high=Math.max(high,arr[i]);}
 if(active.has('support')){for(const p of [pivotHighs.at(-1),pivotLows.at(-1)])if(p){low=Math.min(low,p.price);high=Math.max(high,p.price);}}
 if(active.has('fib')){const pp=latestSwingPair();if(pp)for(const v of fibValues(pp)) {low=Math.min(low,v.price);high=Math.max(high,v.price);}}
 if(!Number.isFinite(low)||!Number.isFinite(high)){low=0;high=1;}if(low===high){const pad=Math.max(Math.abs(low)*.005,1);low-=pad;high+=pad;}const margin=(high-low)*.06;low-=margin;high+=margin;
 const yPrice=v=>top+(high-v)/(high-low)*priceH;const xIndex=i=>left+(i-start+.5)*candleW;
 layout={w,h,left,right,top,bottom,volTop,volBottom,panelTop,panelBottom,plotW,priceH,candleW,low,high,yPrice,xIndex,start,end,profileOn};
 const profileKey=`${start}:${end}`;if(profileOn){if(profileCacheKey!==profileKey){profileCache=calcVisibleProfile(start,end);profileCacheKey=profileKey;}}else{profileCache=null;profileCacheKey='';}
 // Session blocks, kept deliberately subtle because UTC sessions overlap.
 if(active.has('sessions')){const sessionColors=['#49a8c41a','#aa8bea16','#e9ad5716'];for(let i=start;i<end;i++){const session=sessionCode(i);if(session<0)continue;ctx.fillStyle=sessionColors[session];ctx.fillRect(left+(i-start)*candleW,top,candleW+1,priceH);}}
 ctx.font='10px ui-monospace,SFMono-Regular,Menlo,monospace';ctx.textBaseline='middle';
 const xTicks=Math.max(2,Math.floor(plotW/140));
 for(let k=0;k<=xTicks;k++){const x=left+plotW*k/xTicks;ctx.strokeStyle='#14262d';ctx.lineWidth=1;ctx.beginPath();ctx.moveTo(x,top);ctx.lineTo(x,bottom);ctx.stroke();}
 for(let k=0;k<=5;k++){const y=top+priceH*k/5,val=high-(high-low)*k/5;ctx.strokeStyle=COLORS.grid;ctx.lineWidth=1;ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.fillStyle=COLORS.text;ctx.textAlign='right';ctx.fillText(formatPrice(val),left-9,y);}
 // Current visible range: price-envelope zones.
 if(active.has('fvg'))for(const z of fvgZones){if(z.end<start||z.start>=end)continue;const x1=left+(Math.max(start,z.start)-start)*candleW,x2=left+(Math.min(end,z.end+1)-start)*candleW;ctx.fillStyle=z.kind==='up'?'#42d6a018':'#ff687a18';ctx.fillRect(x1,yPrice(z.top),Math.max(1,x2-x1),Math.max(1,yPrice(z.bottom)-yPrice(z.top)));ctx.strokeStyle=z.kind==='up'?'#42d6a045':'#ff687a45';ctx.strokeRect(x1,yPrice(z.top),Math.max(1,x2-x1),Math.max(1,yPrice(z.bottom)-yPrice(z.top)));}
 if(active.has('order-blocks'))for(const z of orderBlocks){if(z.end<start||z.start>=end)continue;const x1=left+(Math.max(start,z.start)-start)*candleW,x2=left+(Math.min(end,z.end+1)-start)*candleW;ctx.fillStyle=z.kind==='up'?'#49c6bc15':'#d988b515';ctx.fillRect(x1,yPrice(z.top),Math.max(1,x2-x1),Math.max(1,yPrice(z.bottom)-yPrice(z.top)));}
 // Candles / line view. Wicks retain full high-low detail.
 if(viewMode==='line'){
   ctx.beginPath();let begun=false;for(let i=start;i<end;i++){const x=xIndex(i),y=yPrice(C[i].close);if(!begun){ctx.moveTo(x,y);begun=true;}else ctx.lineTo(x,y);}ctx.strokeStyle=COLORS.cyan;ctx.lineWidth=1.7;ctx.stroke();
 }else{
   const bodyW=Math.max(1,Math.min(16,candleW*.64));
   for(let i=start;i<end;i++){
     const c=currentViewCandle(i);const up=viewMode==='hollow'?c.close>=(i?C[i-1].close:c.open):c.close>=c.open;const col=up?COLORS.green:COLORS.red;const x=xIndex(i),yo=yPrice(c.open),yc=yPrice(c.close),yh=yPrice(c.high),yl=yPrice(c.low);
     ctx.strokeStyle=col;ctx.lineWidth=Math.max(1,Math.min(1.6,candleW*.16));ctx.beginPath();ctx.moveTo(x,yh);ctx.lineTo(x,yl);ctx.stroke();
     const by=Math.min(yo,yc),bh=Math.max(1,Math.abs(yc-yo));ctx.lineWidth=Math.max(1,Math.min(1.3,candleW*.13));if(viewMode==='hollow'&&up){ctx.strokeRect(x-bodyW/2,by,bodyW,bh);}else{ctx.fillStyle=col;ctx.fillRect(x-bodyW/2,by,bodyW,bh);}
   }
 }
 // Price overlays.
 function lineSeries(arr,color,width=1.25,dash=[]){ctx.save();ctx.strokeStyle=color;ctx.lineWidth=width;ctx.setLineDash(dash);ctx.beginPath();let pen=false;for(let i=start;i<end;i++){const v=arr?.[i];if(!Number.isFinite(v)){pen=false;continue;}const x=xIndex(i),y=yPrice(v);if(!pen){ctx.moveTo(x,y);pen=true;}else ctx.lineTo(x,y);}ctx.stroke();ctx.restore();}
 function band(upper,lower,color){ctx.save();ctx.fillStyle=color;ctx.beginPath();let began=false;for(let i=start;i<end;i++){if(!Number.isFinite(upper[i]))continue;const x=xIndex(i),y=yPrice(upper[i]);if(!began){ctx.moveTo(x,y);began=true;}else ctx.lineTo(x,y);}for(let i=end-1;i>=start;i--){if(Number.isFinite(lower[i]))ctx.lineTo(xIndex(i),yPrice(lower[i]));}if(began){ctx.closePath();ctx.fill();}ctx.restore();}
 if(active.has('bollinger')){band(bbUpper,bbLower,'#76a8ff12');lineSeries(bbUpper,'#76a8ff9a',1);lineSeries(bbLower,'#76a8ff9a',1);lineSeries(closeSMA20,'#76a8ff',1.1);}
 if(active.has('keltner')){band(keltnerUpper,keltnerLower,'#b49cff0c');lineSeries(keltnerUpper,'#b49cff75',1,[4,3]);lineSeries(keltnerLower,'#b49cff75',1,[4,3]);lineSeries(closeEMA20,'#b49cff',1.1);}
 if(active.has('donchian')){lineSeries(donchHi,'#f4bd6288',1,[3,3]);lineSeries(donchLo,'#f4bd6288',1,[3,3]);}
 if(active.has('sma20'))lineSeries(closeSMA20,COLORS.blue,1.45);
 if(active.has('ema20'))lineSeries(closeEMA20,COLORS.amber,1.35);
 if(active.has('ema50'))lineSeries(closeEMA50,COLORS.purple,1.35);
 if(active.has('vwap'))lineSeries(vwap,COLORS.cyan,1.5);
 if(active.has('avwap'))lineSeries(avwapSeries,'#6ee7d2',1.6,[5,3]);
 if(active.has('vwma'))lineSeries(vwma20,'#ed90d4',1.3);
 if(active.has('supertrend')){for(let i=start;i<end;i++){if(!Number.isFinite(supertrend[i]))continue;ctx.strokeStyle=superDir[i]>0?COLORS.green:COLORS.red;ctx.lineWidth=1.8;ctx.beginPath();ctx.moveTo(xIndex(i),yPrice(supertrend[i]));if(i+1<end&&Number.isFinite(supertrend[i+1]))ctx.lineTo(xIndex(i+1),yPrice(supertrend[i+1]));ctx.stroke();}}
 if(active.has('ichimoku')){lineSeries(tenkan,COLORS.red,1.05);lineSeries(kijun,COLORS.blue,1.05);}
 if(active.has('atr-stops')){lineSeries(atrLong,'#42d6a077',1,[2,4]);lineSeries(atrShort,'#ff687a77',1,[2,4]);}
 if(active.has('hma21'))lineSeries(HMA21,'#ff9e66',1.6);
 if(active.has('kama'))lineSeries(KAMA,'#7ee787',1.5,[3,2]);
 if(active.has('psar')){for(let i=start;i<end;i++){if(!Number.isFinite(psar[i]))continue;const x=xIndex(i), y=yPrice(psar[i]);ctx.fillStyle=isFinite(psar[i])?(X[i]>psar[i]?'#42d6a0':'#ff687a'):'#42d6a0';ctx.beginPath();ctx.arc(x,y,1.8,0,Math.PI*2);ctx.fill();}}
 if(active.has('support')){
   for(const p of [pivotLows.at(-1),pivotHighs.at(-1)])if(p){const y=yPrice(p.price);ctx.strokeStyle=p.kind==='low'?'#42d6a0a0':'#ff687aa0';ctx.setLineDash([5,4]);ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.setLineDash([]);ctx.fillStyle=p.kind==='low'?COLORS.green:COLORS.red;ctx.textAlign='left';ctx.fillText(`${p.kind==='low'?'S':'R'} ${formatPrice(p.price)}`,left+5,y-8);}}
 if(active.has('fib')){const pp=latestSwingPair();if(pp){for(const f of fibValues(pp)){const y=yPrice(f.price);ctx.strokeStyle='#f4bd6270';ctx.setLineDash([2,4]);ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.setLineDash([]);ctx.fillStyle=COLORS.amber;ctx.textAlign='left';ctx.fillText(`${f.label} ${formatPrice(f.price)}`,left+5,y-8);}}}
 if(active.has('gaps'))for(const g of gapEvents){if(g.i<start||g.i>=end)continue;const x=xIndex(g.i);ctx.strokeStyle=g.kind==='up'?COLORS.green:COLORS.red;ctx.setLineDash([2,2]);ctx.beginPath();ctx.moveTo(x,yPrice(g.top));ctx.lineTo(x,yPrice(g.bottom));ctx.stroke();ctx.setLineDash([]);}
 if(active.has('swing-pivots'))for(const p of pivots){if(p.i<start||p.i>=end)continue;ctx.fillStyle=p.kind==='high'?COLORS.red:COLORS.green;const x=xIndex(p.i),y=yPrice(p.price)+(p.kind==='high'?-5:5);ctx.beginPath();ctx.arc(x,y,2.5,0,Math.PI*2);ctx.fill();}
 if(active.has('structure'))for(const e of structureEvents){if(e.i<start||e.i>=end)continue;const x=xIndex(e.i),up=e.kind.endsWith('up');ctx.fillStyle=up?COLORS.green:COLORS.red;ctx.beginPath();ctx.moveTo(x,yPrice(e.price)+(up?8:-8));ctx.lineTo(x-4,yPrice(e.price)+(up?-1:1));ctx.lineTo(x+4,yPrice(e.price)+(up?-1:1));ctx.closePath();ctx.fill();ctx.font='8px ui-monospace,monospace';ctx.textAlign='center';ctx.fillText(e.kind.startsWith('choch')?'CH':'BOS',x,yPrice(e.price)+(up?19:-19));}
 if(active.has('equal-levels'))for(const e of equalEvents){if(e.i<start||e.i>=end)continue;ctx.fillStyle=COLORS.amber;ctx.beginPath();ctx.arc(xIndex(e.i),yPrice(e.price),3,0,Math.PI*2);ctx.fill();}
 if(active.has('sweeps'))for(const e of sweepEvents){if(e.i<start||e.i>=end)continue;ctx.strokeStyle=COLORS.amber;ctx.lineWidth=1.5;ctx.beginPath();ctx.arc(xIndex(e.i),yPrice(e.price),4,0,Math.PI*2);ctx.stroke();}
 if(active.has('breaker-blocks'))for(const e of breakerEvents){if(e.i<start||e.i>=end)continue;ctx.fillStyle=COLORS.purple;ctx.fillRect(xIndex(e.i)-3,yPrice(e.price)-3,6,6);}
 // Pattern scanner hits: all matches are retained; visible hits get a badge dot.
 if(active.has('patterns')){
   const byBar=new Map();for(const p of PATTERNS){if(p.end<start||p.end>=end||!C[p.end])continue;const group=byBar.get(p.end)||[];group.push(p);byBar.set(p.end,group);}
   for(const [i,matches] of byBar){const bulls=matches.filter(p=>p.bias.toLowerCase().includes('bull')).length,bears=matches.filter(p=>p.bias.toLowerCase().includes('bear')).length,bull=bulls>bears,bear=bears>bulls,y=bull?yPrice(L[i])+10:yPrice(H[i])-10,x=xIndex(i),r=3.2+Math.min(2,Math.log2(matches.length));ctx.fillStyle=bull?COLORS.green:bear?COLORS.red:COLORS.cyan;ctx.beginPath();ctx.arc(x,y,r,0,Math.PI*2);ctx.fill();if(matches.length>1){ctx.fillStyle='#071116';ctx.font='bold 8px ui-monospace,monospace';ctx.textAlign='center';ctx.fillText(String(matches.length),x,y+.5);}}
 }
 // Live-edge guide: a crisp last-price rail and a matching axis tag.
 if(N-1>=start&&N-1<end){const lastY=yPrice(C[N-1].close),lastUp=C[N-1].close>=(N>1?C[N-2].close:C[N-1].open),tone=lastUp?COLORS.green:COLORS.red;ctx.save();ctx.strokeStyle=lastUp?'#42d6a088':'#ff687a88';ctx.setLineDash([3,4]);ctx.beginPath();ctx.moveTo(left,lastY);ctx.lineTo(right,lastY);ctx.stroke();ctx.setLineDash([]);ctx.fillStyle=tone;ctx.beginPath();ctx.arc(xIndex(N-1),lastY,3,0,Math.PI*2);ctx.fill();ctx.fillStyle=tone;ctx.fillRect(1,lastY-8,left-7,16);ctx.fillStyle='#071116';ctx.font='9px ui-monospace,monospace';ctx.textAlign='right';ctx.textBaseline='middle';ctx.fillText(formatPrice(C[N-1].close),left-5,lastY);ctx.restore();}
 // Volume panel, still at candle resolution.
 ctx.fillStyle='#0b171c';ctx.fillRect(left,volTop,plotW,volH);let maxVol=1;for(let i=start;i<end;i++)maxVol=Math.max(maxVol,V[i]);
 for(let i=start;i<end;i++){const x=xIndex(i),v=Math.max(0,V[i]),bar=Math.max(1,v/maxVol*(volH-4));ctx.fillStyle=C[i].close>=C[i].open?'#42d6a075':'#ff687a75';ctx.fillRect(x-Math.max(.5,candleW*.32),volBottom-bar,Math.max(1,candleW*.64),bar);}
 ctx.fillStyle=COLORS.text;ctx.textAlign='left';ctx.font='9px ui-monospace,monospace';ctx.fillText('VOL',left+4,volTop+9);
 // Visible-range profile: distribute each bar's volume across its high-low envelope, never claim tick accuracy.
 if(profileOn&&profileCache){const p=profileCache,maxBar=Math.max(1,p.max);for(let k=0;k<p.vol.length;k++){const priceLo=p.lo+k*p.step,priceHi=priceLo+p.step,y1=yPrice(priceHi),y2=yPrice(priceLo),width=80*p.vol[k]/maxBar;ctx.fillStyle=k>=p.low&&k<=p.high?'#48c8d855':'#48c8d82a';ctx.fillRect(right+6,y1,Math.max(1,width),Math.max(1,y2-y1));}ctx.fillStyle=COLORS.text;ctx.font='8px ui-monospace,monospace';ctx.textAlign='left';ctx.fillText('VOL PROFILE',right+5,top-9);if(active.has('profile-value')){const poc=p.price(p.poc),vah=p.price(p.high),val=p.price(p.low);for(const [price,label,color] of [[poc,'POC',COLORS.amber],[vah,'VAH',COLORS.cyan],[val,'VAL',COLORS.cyan]]){const y=yPrice(price);ctx.strokeStyle=color;ctx.setLineDash(label==='POC'?[]:[3,3]);ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right+89,y);ctx.stroke();ctx.setLineDash([]);ctx.fillStyle=color;ctx.fillText(`${label} ${formatPrice(price)}`,right+7,Math.max(top+7,Math.min(bottom-5,y-8)));}}}
 // Optional lower indicator panel: at most one oscillator/flow series at a time.
 if(panelId&&PANELS[panelId])drawPanel(panelId,left,right,panelTop,panelBottom,start,end);
 // Two-step R-multiple ruler: drag entry to stop, then click a target price.
 if(active.has('ruler')&&ruler){ctx.save();ctx.setLineDash([4,3]);ctx.lineWidth=1.2;ctx.strokeStyle=COLORS.amber;ctx.beginPath();ctx.moveTo(ruler.entryX,ruler.entryY);ctx.lineTo(ruler.stopX,ruler.stopY);ctx.stroke();if(ruler.targetPrice!=null){const risk=Math.abs(ruler.entryPrice-ruler.stopPrice),direction=ruler.stopPrice<ruler.entryPrice?1:-1,rMultiple=risk?direction*(ruler.targetPrice-ruler.entryPrice)/risk:0;const targetY=yPrice(ruler.targetPrice);ctx.strokeStyle=rMultiple>=0?COLORS.green:COLORS.red;ctx.beginPath();ctx.moveTo(ruler.entryX,targetY);ctx.lineTo(right,targetY);ctx.stroke();ctx.fillStyle=rMultiple>=0?COLORS.green:COLORS.red;ctx.fillText(`Target ${rMultiple>=0?'+':''}${rMultiple.toFixed(2)}R · ${formatPct((ruler.targetPrice-ruler.entryPrice)/ruler.entryPrice)}`,Math.min(right-145,ruler.entryX+8),targetY-8);}else{ctx.fillStyle=COLORS.amber;ctx.fillText(`Risk ${formatPrice(Math.abs(ruler.entryPrice-ruler.stopPrice))} · ${formatPct((ruler.stopPrice-ruler.entryPrice)/ruler.entryPrice)}`,ruler.stopX+7,ruler.stopY-8);}ctx.restore();}
 // Crosshair and time/price hover labels.
 if(hoverIndex>=start&&hoverIndex<end&&layout){const x=xIndex(hoverIndex),c=C[hoverIndex];ctx.strokeStyle='#d0e4e65e';ctx.setLineDash([3,4]);ctx.beginPath();ctx.moveTo(x,top);ctx.lineTo(x,panelId?panelBottom:volBottom);ctx.stroke();ctx.setLineDash([]);const y=yPrice(c.close);ctx.strokeStyle=COLORS.cyan;ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.fillStyle=COLORS.cyan;ctx.fillRect(1,y-8,left-6,16);ctx.fillStyle='#041014';ctx.font='9px ui-monospace,monospace';ctx.textAlign='right';ctx.fillText(formatPrice(c.close),left-5,y);}
 // X-axis labels.
 ctx.font='9px ui-monospace,SFMono-Regular,monospace';ctx.textAlign='center';ctx.textBaseline='top';const ticks=xTicks;for(let k=0;k<=ticks;k++){const i=Math.min(end-1,start+Math.round((span-1)*k/ticks));const x=xIndex(i);ctx.strokeStyle=COLORS.grid;ctx.beginPath();ctx.moveTo(x,volBottom);ctx.lineTo(x,volBottom+4);ctx.stroke();ctx.fillStyle=COLORS.text;ctx.fillText(axisTimeText(i,span),x,Math.min(h-15,volBottom+7));}
 if(profileOn){ctx.fillStyle='#719097';ctx.font='8px ui-monospace,monospace';ctx.textAlign='right';ctx.fillText('OHLCV allocation',w-7,h-8);}
 updateBarReadout(hoverIndex);updateStats();
}
function latestSwingPair(){const high=pivotHighs.at(-1),low=pivotLows.at(-1);if(!high||!low)return null;return high.i>low.i?{from:high,to:low}:{from:low,to:high};}
function fibValues(pair){const a=pair.from.price,b=pair.to.price,d=b-a;return[.382,.5,.618,.786].map(r=>({label:r.toFixed(3),price:b-d*r}));}
function drawPanel(id,left,right,top,bottom,a,b){
 const m=PANELS[id];if(!m)return;const xStart=left,xEnd=right,width=xEnd-xStart,height=Math.max(40,bottom-top);ctx.fillStyle='#0a171c';ctx.fillRect(xStart,top,width,height);ctx.strokeStyle=COLORS.grid;ctx.beginPath();ctx.moveTo(xStart,top);ctx.lineTo(xEnd,top);ctx.stroke();
 const arrays=[m.a,m.b,m.c,m.hist].filter(Boolean);let min=Infinity,max=-Infinity;
 if(m.range){min=m.range[0]==null?Infinity:m.range[0];max=m.range[1]==null?-Infinity:m.range[1];}
 for(const arr of arrays)for(let i=a;i<b;i++){const v=arr[i];if(Number.isFinite(v)){min=Math.min(min,v);max=Math.max(max,v);}}
 if(!Number.isFinite(min)||!Number.isFinite(max)){min=0;max=1;}if(min===max){min-=1;max+=1;}const pad=(max-min)*.08;min-=pad;max+=pad;
 const py=v=>top+(max-v)/(max-min)*height,px=i=>left+(i-a+.5)*(width/(b-a));
 for(const g of m.guides||[]){const y=py(g);ctx.strokeStyle='#82979d35';ctx.setLineDash([3,4]);ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.setLineDash([]);}
 if(m.zero||min<0&&max>0){const y=py(0);ctx.strokeStyle='#91a9ad44';ctx.setLineDash([2,3]);ctx.beginPath();ctx.moveTo(left,y);ctx.lineTo(right,y);ctx.stroke();ctx.setLineDash([]);}
 if(m.hist){for(let i=a;i<b;i++){const v=m.hist[i];if(!Number.isFinite(v))continue;const y=py(v),zero=py(0);ctx.fillStyle=v>=0?'#42d6a077':'#ff687a77';ctx.fillRect(px(i)-Math.max(.5,width/(b-a)*.27),Math.min(y,zero),Math.max(1,width/(b-a)*.54),Math.max(1,Math.abs(y-zero)));}}
 const cols=[COLORS.cyan,COLORS.amber,COLORS.purple];arrays.filter(x=>x!==m.hist).forEach((arr,idx)=>{ctx.strokeStyle=cols[idx%cols.length];ctx.lineWidth=1.25;ctx.beginPath();let pen=false;for(let i=a;i<b;i++){if(!Number.isFinite(arr[i])){pen=false;continue;}const x=px(i),y=py(arr[i]);if(!pen){ctx.moveTo(x,y);pen=true;}else ctx.lineTo(x,y);}ctx.stroke();});
 ctx.fillStyle=COLORS.text;ctx.font='9px ui-monospace,monospace';ctx.textAlign='left';ctx.textBaseline='top';ctx.fillText(`${m.name}   ${m.fmt(last(m.a))}`,left+5,top+4);ctx.textAlign='right';ctx.fillText(formatPrice(max),left-8,top+8);ctx.fillText(formatPrice(min),left-8,bottom-8);
}
function updateBarReadout(i){
 if(!N)return;const index=i>=start&&i<end?i:N-1,c=C[index],d=new Date(c.time),stamp=Number.isFinite(d.getTime())?d.toLocaleString(undefined,{timeZone:'UTC',month:'short',day:'2-digit',hour:'2-digit',minute:'2-digit',hour12:false})+' UTC':`bar ${index+1}`;
 document.getElementById('barTime').textContent=`${index===N-1?'LATEST':'BAR '+(index+1)} · ${stamp}`;
 document.getElementById('readOpen').textContent=formatPrice(c.open);document.getElementById('readHigh').textContent=formatPrice(c.high);document.getElementById('readLow').textContent=formatPrice(c.low);document.getElementById('readClose').textContent=formatPrice(c.close);document.getElementById('readVolume').textContent=formatVolume(c.volume);
 const change=c.open?c.close/c.open-1:0,wrap=document.getElementById('readChangeWrap');wrap.classList.toggle('positive',change>=0);wrap.classList.toggle('negative',change<0);document.getElementById('readChange').textContent=formatPct(change);
}
function updateTooltip(i,point){
 if(i<0||!C[i]){tooltip.style.display='none';updateBarReadout(-1);return;}const c=C[i],patterns=PATTERNS.filter(p=>p.end===i);const bodyPct=c.open?(c.close/c.open-1):0;updateBarReadout(i);
 tooltip.innerHTML=`<div class="tooltip-head"><span>${escapeHtml(timeText(i,true))}</span><span>${i+1}/${N}</span></div><div class="tooltip-grid"><span>Open<b>${formatPrice(c.open)}</b></span><span>High<b>${formatPrice(c.high)}</b></span><span>Low<b>${formatPrice(c.low)}</b></span><span>Close<b>${formatPrice(c.close)}</b></span><span>Volume<b>${formatVolume(c.volume)}</b></span><span>Body<b>${formatPct(bodyPct)}</b></span><span>Range<b>${formatPrice(c.high-c.low)}</b></span><span>RVOL20<b>${PANELS.rvol.a[i]==null?'—':PANELS.rvol.a[i].toFixed(2)+'×'}</b></span><span>RSI14<b>${rsi[i]==null?'—':rsi[i].toFixed(1)}</b></span><span>ATR14<b>${ATR14[i]==null?'—':formatPrice(ATR14[i])}</b></span><span>CMF20<b>${cmf[i]==null?'—':cmf[i].toFixed(3)}</b></span><span>Delta proxy<b>${formatVolume(delta[i])}</b></span><span>HMA21<b>${HMA21[i]==null?'—':formatPrice(HMA21[i])}</b></span><span>KAMA<b>${KAMA[i]==null?'—':formatPrice(KAMA[i])}</b></span><span>Williams %R<b>${willr[i]==null?'—':willr[i].toFixed(1)}</b></span><span>ROC12<b>${roc12[i]==null?'—':roc12[i].toFixed(1)+'%'}</b></span><span>AroonUp<b>${aroonUp[i]==null?'—':aroonUp[i].toFixed(0)}</b></span><span>Coppock<b>${coppock[i]==null?'—':coppock[i].toFixed(1)}</b></span></div>${patterns.length?`<div class="tooltip-patterns"><strong>${patterns.length} pattern match${patterns.length===1?'':'es'}</strong><br>${patterns.map(p=>`${escapeHtml(p.type)} · ${escapeHtml(p.bias)}`).join('<br>')}</div>`:''}`;
 tooltip.style.display='block';const shellRect=shell.getBoundingClientRect(),x=point?point.x:layout.xIndex(i),y=point?point.y:layout.top+10;const tw=tooltip.offsetWidth,th=tooltip.offsetHeight;tooltip.style.left=Math.max(5,Math.min(shellRect.width-tw-6,x+14))+'px';tooltip.style.top=Math.max(5,Math.min(shellRect.height-th-6,y+12))+'px';
}
function indexAt(x){if(!layout)return-1;return Math.max(start,Math.min(end-1,Math.floor(start+(x-layout.left)/layout.plotW*(end-start))));}
function priceAt(y){if(!layout)return null;return layout.high-(y-layout.top)/(layout.bottom-layout.top)*(layout.high-layout.low);}
function localPoint(e){const r=chart.getBoundingClientRect();return{x:e.clientX-r.left,y:e.clientY-r.top};}
chart.addEventListener('pointerdown',e=>{
 const p=localPoint(e),idx=indexAt(p.x);if(idx<0)return;
 if(armAnchor){anchorIndex=idx;avwapSeries=anchoredVwap(anchorIndex);SERIES.avwap=avwapSeries;armAnchor=false;draw();updateStats();document.getElementById('footStatus').textContent=`Anchored VWAP from ${timeText(idx,true)} · click Anchor VWAP to move it.`;return;}
 if(active.has('ruler')){if(ruler?.stage==='target'){ruler.targetPrice=priceAt(p.y);ruler.targetX=p.x;ruler.stage='done';document.getElementById('footStatus').textContent='R multiple measured from entry-to-stop risk. Drag again to start a new ruler.';draw();return;}const price=priceAt(p.y);ruler={entryX:p.x,entryY:p.y,stopX:p.x,stopY:p.y,entryPrice:price,stopPrice:price,targetPrice:null,stage:'stop'};dragState={mode:'ruler-stop',pointerId:e.pointerId};chart.setPointerCapture(e.pointerId);draw();return;}
 dragState={mode:'pan',pointerId:e.pointerId,x:p.x,start,end,changed:false};chart.setPointerCapture(e.pointerId);
});
chart.addEventListener('pointermove',e=>{
 const p=localPoint(e);
 if(dragState?.mode==='pan'){
   const bars=Math.round((dragState.x-p.x)/(layout?.candleW||1)),span=dragState.end-dragState.start;let nextStart=dragState.start+bars,nextEnd=dragState.end+bars;if(nextStart<0){nextEnd-=nextStart;nextStart=0;}if(nextEnd>N){nextStart-=nextEnd-N;nextEnd=N;}nextStart=Math.max(0,nextStart);if(nextStart!==start||nextEnd!==end){start=nextStart;end=nextEnd;dragState.changed=true;draw();updateStats();}return;
 }
 if(dragState?.mode==='ruler-stop'&&ruler){ruler.stopX=p.x;ruler.stopY=p.y;ruler.stopPrice=priceAt(p.y);draw();return;}
 hoverIndex=indexAt(p.x);draw();updateTooltip(hoverIndex,p);
});
function endPointer(e){if(dragState?.pointerId===e.pointerId){if(dragState.mode==='ruler-stop'&&ruler){ruler.stage='target';document.getElementById('footStatus').textContent='Risk leg set. Now click a target price to calculate the R multiple.';}dragState=null;} }
chart.addEventListener('pointerup',endPointer);chart.addEventListener('pointercancel',endPointer);
chart.addEventListener('pointerleave',()=>{if(!dragState){hoverIndex=-1;tooltip.style.display='none';draw();}});
chart.addEventListener('wheel',e=>{e.preventDefault();const p=localPoint(e),at=indexAt(p.x);zoomAt(e.deltaY>0?1.15:.87,at);},{passive:false});
chart.addEventListener('dblclick',fitAll);
window.addEventListener('resize',()=>{renderLegend();draw();});

document.getElementById('viewMode').addEventListener('change',e=>{viewMode=e.target.value;renderLegend();draw();});
document.querySelectorAll('[data-window]').forEach(b=>b.addEventListener('click',()=>setWindowCount(b.dataset.window)));
document.getElementById('fitTop').addEventListener('click',fitAll);
document.getElementById('zoomIn').addEventListener('click',()=>zoomAt(.8));
document.getElementById('zoomOut').addEventListener('click',()=>zoomAt(1.25));
document.getElementById('anchorButton').addEventListener('click',()=>{armAnchor=!armAnchor;document.getElementById('footStatus').textContent=armAnchor?'Click a candle to set the anchored-VWAP starting point.':'Anchor selection cancelled.';updateStats();});
document.getElementById('featureSearch').addEventListener('input',e=>{featureQuery=e.target.value;renderFeatureList();});
for(const id of ['sizeEquity','sizeRisk','sizeEntry','sizeDistance'])document.getElementById(id).addEventListener('input',updateSizeCalculator);
function savePng(){try{const a=document.createElement('a');a.download=`${String(DATA.symbol||'market').replace(/[^a-z0-9_-]/gi,'-')}-chart.png`;a.href=chart.toDataURL('image/png');a.click();}catch(e){document.getElementById('footStatus').textContent='PNG export was blocked by this browser.';}}
document.getElementById('exportPng').addEventListener('click',savePng);document.getElementById('exportPngSmall').addEventListener('click',savePng);
document.getElementById('openFeatures').addEventListener('click',()=>document.getElementById('sidebar').classList.toggle('open'));
document.addEventListener('keydown',e=>{if(['INPUT','SELECT','TEXTAREA'].includes(document.activeElement?.tagName))return;if(e.key==='ArrowLeft'){const d=e.shiftKey?10:1;start=Math.max(0,start-d);end=Math.max(start+1,end-d);draw();updateStats();}else if(e.key==='ArrowRight'){const d=e.shiftKey?10:1;start=Math.min(Math.max(0,N-(end-start)),start+d);end=Math.min(N,end+d);draw();updateStats();}else if(e.key==='+'||e.key==='=')zoomAt(.8);else if(e.key==='-')zoomAt(1.25);else if(e.key.toLowerCase()==='f')fitAll();else if(e.key==='Home')fitAll();});

function init(){
 const isFull=Boolean(DATA.fullClient||DATA.full||DATA.client||DATA.fullClientApp);
 const badge=document.getElementById('fullClientBadge');if(badge){badge.style.display=isFull?'inline-flex':'none';badge.classList.toggle('full',isFull);badge.textContent=isFull?'FULL CLIENT APP · 60 ULTRA STUDIES':'LITE CHART · add --full for 60 studies';badge.title=isFull?'Full client app enabled via --full / --client / --full-client / --app':'Enable full client app with --full (aliases: --client, --full-client, --app)';}
 document.getElementById('symbol').textContent=DATA.symbol||'Market';document.getElementById('interval').textContent=DATA.interval||'Candles';document.title=`ztrade · ${DATA.symbol||'Chart'} · Flow Lens${isFull?' · Full Client':''}`;
 document.documentElement.setAttribute('data-full-client', String(isFull));
 if(isFull) document.body.classList.add('full-client');
 renderFeatureList();renderPatternLedger();updateStats();
 if(N){document.getElementById('sizeEntry').value=String(C[N-1].close);document.getElementById('sizeDistance').value=String((last(ATR14)||Math.abs(C[N-1].close)*.01)*2);}
 draw();
}
init();
</script>
</body>
</html>
""";
}
