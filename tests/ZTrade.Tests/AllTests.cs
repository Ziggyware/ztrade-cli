using System.Net;
using ZTrade.Cli;
using ZTrade.Core.Indicators;
using ZTrade.Core.Market;
using ZTrade.Core.Patterns;
using ZTrade.Exchanges;
using ZTrade.Exchanges.Gemini;
using ZTrade.Trading;
using static ZTrade.Tests.T;

namespace ZTrade.Tests;

internal static class AllTests
{
    private static readonly PatternOptions Opt = new();

    public static IEnumerable<(string Name, Func<Task> Test)> Collect()
    {
        // ---- Candle -----------------------------------------------------
        yield return ("Candle: derived geometry", () => Sync(() =>
        {
            var c = C(0, 10, 12, 8, 11);
            Check.Eq(1m, c.Body); Check.Eq(1m, c.UpperShadow); Check.Eq(2m, c.LowerShadow);
            Check.Eq(4m, c.Range); Check.True(c.IsBullish); Check.Eq(10.5m, c.BodyMidpoint);
        }));
        yield return ("Candle: rejects broken OHLC / non-positive price / negative volume", () => Sync(() =>
        {
            Check.Throws<ArgumentException>(() => C(0, 10, 9, 8, 11));   // high < close
            Check.Throws<ArgumentException>(() => C(0, 10, 12, 10.5m, 11)); // low > open
            Check.Throws<ArgumentOutOfRangeException>(() => C(0, 0, 1, 0, 1));
            Check.Throws<ArgumentOutOfRangeException>(() => new Candle(0, 1, 1, 1, 1, -1));
        }));
        yield return ("CandleSeries: rejects non-ascending", () => Sync(() =>
        {
            Check.Throws<ArgumentException>(() => CandleSeries.EnsureAscending(new[] { C(2, 1, 1, 1, 1), C(1, 1, 1, 1, 1) }));
        }));
        yield return ("Interval: parse round-trip", () => Sync(() =>
        {
            foreach (var i in Enum.GetValues<CandleInterval>())
            {
                Check.True(CandleIntervalExtensions.TryParse(i.ToShortString(), out var parsed) && parsed == i, i.ToString());
            }

            Check.True(!CandleIntervalExtensions.TryParse("2h", out _));
        }));

        // ---- Indicators -------------------------------------------------
        yield return ("SMA: values and warm-up", () => Sync(() =>
        {
            var r = new Sma(3).Run(new decimal[] { 1, 2, 3, 4, 5 });
            Check.True(r[0] is null && r[1] is null);
            Check.Eq(2m, r[2]!.Value); Check.Eq(3m, r[3]!.Value); Check.Eq(4m, r[4]!.Value);
        }));
        yield return ("SMA: no drift over 100k decimal updates", () => Sync(() =>
        {
            var sma = new Sma(7); decimal? last = null;
            for (var i = 0; i < 100_000; i++) last = sma.Update(0.1m + (i % 3) * 0.01m);
            Check.Near(0.11m, last!.Value, 0.0035m); // mean of any 7 consecutive values from {0.10,0.11,0.12}
        }));
        yield return ("EMA: seeded by SMA, then recursive", () => Sync(() =>
        {
            var r = new Ema(3).Run(new decimal[] { 1, 2, 3, 4 });
            Check.Eq(2m, r[2]!.Value); Check.Eq(3m, r[3]!.Value);
            Check.Eq(5m, new Ema(3).Run(new decimal[] { 5, 5, 5, 5 })[3]!.Value);
        }));
        yield return ("RSI: boundaries and Wilder smoothing", () => Sync(() =>
        {
            Check.Eq(100m, new Rsi(3).Run(new decimal[] { 1, 2, 3, 4, 5 })[4]!.Value);
            Check.Eq(50m, new Rsi(3).Run(new decimal[] { 5, 5, 5, 5, 5 })[4]!.Value);
            var r = new Rsi(2).Run(new decimal[] { 10, 11, 10, 12 });
            Check.Eq(50m, r[2]!.Value);
            Check.Near(83.3333m, r[3]!.Value, 0.001m);
        }));

        // ---- Patterns ---------------------------------------------------
        yield return ("Pattern: doji family", () => Sync(() =>
        {
            Check.True(CandlePatterns.IsLongLeggedDoji(C(0, 10, 11, 9, 10.01m), Opt));
            Check.True(CandlePatterns.IsDragonflyDoji(C(0, 10, 10, 8, 10), Opt));
            Check.True(CandlePatterns.IsGravestoneDoji(C(0, 10, 12, 10, 10), Opt));
            Check.True(!CandlePatterns.IsDoji(C(0, 10, 11, 9, 10.9m), Opt));
            Check.True(!CandlePatterns.IsDoji(C(0, 10, 10, 10, 10), Opt), "flat candle has no range");
        }));
        yield return ("Pattern: engulfing", () => Sync(() =>
        {
            Check.True(CandlePatterns.IsBullishEngulfing(C(0, 10, 10.2m, 8.8m, 9), C(1, 8.9m, 10.6m, 8.8m, 10.5m)));
            Check.True(!CandlePatterns.IsBullishEngulfing(C(0, 10, 10.2m, 8.8m, 9), C(1, 9.2m, 10.6m, 9.1m, 10.5m)));
            Check.True(CandlePatterns.IsBearishEngulfing(C(0, 9, 10.2m, 8.8m, 10), C(1, 10.1m, 10.2m, 8.5m, 8.6m)));
        }));
        yield return ("Pattern: harami is contained, piercing is midpoint-based (legacy bugs not ported)", () => Sync(() =>
        {
            var prev = C(0, 10, 10.1m, 7.9m, 8);
            Check.True(CandlePatterns.IsBullishHarami(prev, C(1, 8.5m, 9.7m, 8.4m, 9.5m), Opt));
            Check.True(!CandlePatterns.IsBullishHarami(prev, C(1, 7.5m, 10.5m, 7.4m, 10.4m), Opt), "engulfing is not a harami");
            Check.True(CandlePatterns.IsBullishPiercing(prev, C(1, 7.8m, 9.6m, 7.7m, 9.5m)));
            Check.True(!CandlePatterns.IsBullishPiercing(prev, C(1, 7.8m, 10.6m, 7.7m, 10.5m)), "closing above prior open is engulfing");
            Check.True(!CandlePatterns.IsBullishPiercing(prev, C(1, 7.8m, 8.9m, 7.7m, 8.8m)), "below midpoint is not piercing");
        }));
        yield return ("Pattern: dark cloud cover", () => Sync(() =>
        {
            Check.True(CandlePatterns.IsBearishDarkCloudCover(C(0, 8, 10.1m, 7.9m, 10), C(1, 10.2m, 10.3m, 8.4m, 8.5m)));
        }));
        yield return ("Pattern: morning star / evening star", () => Sync(() =>
        {
            Check.True(CandlePatterns.IsBullishMorningStar(C(0, 20, 20.5m, 9.5m, 10), C(1, 9.5m, 9.8m, 8.8m, 9), C(2, 9.2m, 16.2m, 9.1m, 16), Opt));
            Check.True(CandlePatterns.IsBearishEveningStar(C(0, 10, 20.5m, 9.5m, 20), C(1, 20.5m, 21, 20.4m, 21), C(2, 20.8m, 20.9m, 12, 12.5m), Opt));
        }));
        yield return ("Pattern: three white soldiers / black crows", () => Sync(() =>
        {
            Check.True(CandlePatterns.IsBullishThreeWhiteSoldiers(
                C(0, 10, 12.2m, 9.9m, 12), C(1, 11, 14.3m, 10.9m, 14), C(2, 13, 16.4m, 12.9m, 16), Opt));
            Check.True(CandlePatterns.IsBearishThreeBlackCrows(
                C(0, 16, 16.1m, 13.8m, 14), C(1, 15, 15.1m, 11.8m, 12), C(2, 13, 13.1m, 9.9m, 10), Opt));
        }));
        yield return ("Trend: least-squares direction", () => Sync(() =>
        {
            var up = FromCloses(new decimal[] { 1, 2, 3, 4, 5 });
            var down = FromCloses(new decimal[] { 5, 4, 3, 2, 1 });
            var flat = FromCloses(new decimal[] { 3, 3, 3, 3, 3 });
            Check.Eq(TrendDirection.Up, Trend.Direction(up, 0, 5));
            Check.Eq(TrendDirection.Down, Trend.Direction(down, 0, 5));
            Check.Eq(TrendDirection.None, Trend.Direction(flat, 0, 5));
            Check.Eq(TrendDirection.None, Trend.Direction(up, 0, 1));
        }));
        yield return ("Scanner: hammer is bullish after downtrend, hanging man after uptrend, silent without context", () => Sync(() =>
        {
            var hammer = (decimal o, long ts) => C(ts, o, o + 0.25m, o - 2m, o + 0.2m);
            var down = FromCloses(new decimal[] { 20, 19, 18, 17, 16 });
            down.Add(hammer(16, down[^1].Timestamp + 3600));
            Check.True(PatternScanner.Scan(down).Any(m => m.Type == PatternType.BullishHammer && m.EndIndex == 5));

            var up = FromCloses(new decimal[] { 10, 11, 12, 13, 14 });
            up.Add(hammer(14, up[^1].Timestamp + 3600));
            Check.True(PatternScanner.Scan(up).Any(m => m.Type == PatternType.BearishHangingMan));

            var shortHistory = new List<Candle> { hammer(14, 1_700_000_000) };
            Check.True(PatternScanner.Scan(shortHistory).All(m => m.Type != PatternType.BullishHammer && m.Type != PatternType.BearishHangingMan));
        }));
        yield return ("Scanner: matches never reference future candles", () => Sync(() =>
        {
            var series = FromCloses(Enumerable.Range(0, 60).Select(i => 100m + (decimal)Math.Sin(i / 3.0) * 8m));
            var full = PatternScanner.Scan(series);
            var truncated = PatternScanner.Scan(series.Take(40).ToList());
            var fullPrefix = full.Where(m => m.EndIndex < 40).ToList();
            Check.Eq(fullPrefix.Count, truncated.Count, "prefix match count");
            for (var i = 0; i < truncated.Count; i++) Check.Eq(fullPrefix[i], truncated[i]);
        }));

        // ---- Trading ----------------------------------------------------
        yield return ("PaperBroker: fee math and PnL", () => Sync(() =>
        {
            var b = new PaperBroker(1000m, feeRate: 0.01m);
            var buy = b.Buy(1, 100m)!;
            Check.Near(1000m, buy.Quantity * buy.Price + buy.Fee, 0.0000001m, "buy spends exactly cash");
            Check.Eq(0m, b.Cash);
            var sell = b.Sell(2, 110m)!;
            Check.Near(1078.2178m, b.Cash, 0.0001m);
            Check.Near(78.2178m, b.Trades[0].Pnl, 0.0001m);
            Check.True(b.Trades[0].IsWin && sell.Quantity == buy.Quantity);
        }));
        yield return ("PaperBroker: slippage hurts both sides; double buy / sell-when-flat are no-ops", () => Sync(() =>
        {
            var b = new PaperBroker(1000m, 0m, slippageRate: 0.01m);
            Check.Eq(101m, b.Buy(1, 100m)!.Price);
            Check.True(b.Buy(2, 100m) is null);
            Check.Eq(99m, b.Sell(3, 100m)!.Price);
            Check.True(b.Sell(4, 100m) is null);
        }));
        yield return ("Backtest: executes at NEXT open, never same bar; last-bar signal is dropped", () => Sync(() =>
        {
            var candles = new List<Candle>
            {
                C(100, 10, 11, 9, 10), C(200, 20, 21, 19, 20), C(300, 30, 31, 29, 30), C(400, 40, 41, 39, 40),
            };
            var script = new Dictionary<int, Signal> { [0] = Signal.Buy, [2] = Signal.Sell, [3] = Signal.Buy };
            var r = BacktestEngine.Run(candles, new ScriptedStrategy(script), new PaperBroker(1000m, 0m));
            Check.Eq(2, r.Fills.Count);
            Check.Eq(20m, r.Fills[0].Price, "buy at bar1 open"); Check.Eq(200L, r.Fills[0].Timestamp);
            Check.Eq(40m, r.Fills[1].Price, "sell at bar3 open"); Check.Eq(400L, r.Fills[1].Timestamp);
            Check.Eq(2000m, r.FinalEquity); Check.True(!r.EndedInPosition);
            Check.Eq(3m, r.BuyAndHoldReturnFraction, "baseline: 40/10 - 1");
        }));
        yield return ("Backtest: max drawdown and open-position mark-to-market", () => Sync(() =>
        {
            var candles = new List<Candle> { C(1, 100, 101, 99, 100), C(2, 100, 101, 49, 50), C(3, 50, 101, 49, 100) };
            var r = BacktestEngine.Run(candles, new ScriptedStrategy(new() { [0] = Signal.Buy }), new PaperBroker(1000m, 0m));
            Check.Eq(0.5m, r.MaxDrawdownFraction); Check.True(r.EndedInPosition); Check.Eq(1000m, r.FinalEquity);
        }));
        yield return ("SmaCross: silent during warm-up, buys after upturn, sells after downturn", () => Sync(() =>
        {
            var closes = Enumerable.Range(0, 30).Select(i => 100m - i)
                .Concat(Enumerable.Range(1, 40).Select(i => 70m + i * 2))
                .Concat(Enumerable.Range(1, 40).Select(i => 150m - i * 2));
            var candles = FromCloses(closes);
            var s = new SmaCrossStrategy(5, 15);
            var signals = candles.Select(c => s.OnCandle(c)).ToList();
            Check.True(signals.Take(15).All(x => x == Signal.Hold), "warm-up");
            var firstBuy = signals.IndexOf(Signal.Buy);
            var firstSell = signals.IndexOf(Signal.Sell);
            Check.True(firstBuy > 30 && firstSell > firstBuy, $"buy@{firstBuy} sell@{firstSell}");
            Check.Throws<ArgumentException>(() => new SmaCrossStrategy(10, 10));
        }));
        yield return ("RsiReversion: buys on recovery from oversold", () => Sync(() =>
        {
            var closes = Enumerable.Range(0, 20).Select(i => 100m - i * 2).Concat(new decimal[] { 70, 75, 80, 85 });
            var s = new RsiReversionStrategy(5);
            var signals = FromCloses(closes).Select(c => s.OnCandle(c)).ToList();
            Check.True(signals.Contains(Signal.Buy));
        }));

        // ---- Gemini -----------------------------------------------------
        yield return ("Gemini parser: field mapping, ms to seconds, ascending, duplicate timestamps collapse", () => Sync(() =>
        {
            var json = "[" + GeminiRow(1700003600, 101, 104, 100, 103, 7) + "," + GeminiRow(1700000000, 100, 102, 99, 101, 5.5m)
                + "," + GeminiRow(1700000000, 100, 102, 99, 101, 5.5m) + "]";
            var r = GeminiCandleParser.Parse(json);
            Check.Eq(2, r.Count);
            Check.Eq(1700000000L, r[0].Timestamp); Check.Eq(1700003600L, r[1].Timestamp);
            Check.Eq(100m, r[0].Open); Check.Eq(102m, r[0].High); Check.Eq(99m, r[0].Low);
            Check.Eq(101m, r[0].Close); Check.Eq(5.5m, r[0].Volume);
        }));
        yield return ("Gemini parser: malformed input becomes MarketDataException", () => Sync(() =>
        {
            Check.Throws<MarketDataException>(() => GeminiCandleParser.Parse("{\"reason\":\"x\"}"));
            Check.Throws<MarketDataException>(() => GeminiCandleParser.Parse("not json"));
            Check.Throws<MarketDataException>(() => GeminiCandleParser.Parse("[[1,2]]"));
            Check.Throws<MarketDataException>(() => GeminiCandleParser.Parse("[" + GeminiRow(1, 10, 9, 8, 11, 1) + "]"));
        }));
        yield return ("Gemini client: request shape, symbol normalization, argument validation", async () =>
        {
            var h = new FakeHandler((_, _) => FakeHandler.Json(HttpStatusCode.OK, "[" + GeminiRow(1700000000, 1, 2, 1, 2, 5) + "]"));
            var client = NewClient(h);
            var r = await client.GetRecentCandlesAsync("btcusd", CandleInterval.Hour1, 50);
            Check.Eq(1, r.Count);
            Check.Eq("/v2/candles/BTCUSD/1h", h.Requests[0].AbsolutePath);
            await Check.ThrowsAsync<ArgumentException>(() => client.GetRecentCandlesAsync("btc/usd", CandleInterval.Hour1, 5));
            await Check.ThrowsAsync<ArgumentOutOfRangeException>(() => client.GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 0));
            await Check.ThrowsAsync<NotSupportedException>(() => client.GetRecentCandlesAsync("BTCUSD", CandleInterval.Second10, 5));
            Check.Eq(1, h.Requests.Count, "rejected calls make no request");
        });
        yield return ("Gemini client: drops unclosed last candle by default, keeps it on request", async () =>
        {
            var json = "[" + GeminiRow(1700003600, 2, 3, 2, 3, 1) + "," + GeminiRow(1700000000, 1, 2, 1, 2, 1) + "]";
            var now = DateTimeOffset.FromUnixTimeSeconds(1700003600 + 1800);
            var h = new FakeHandler((_, _) => FakeHandler.Json(HttpStatusCode.OK, json));
            Check.Eq(1, (await NewClient(h, now).GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 10)).Count);
            Check.Eq(2, (await NewClient(h, now, includeUnclosed: true).GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 10)).Count);
        });
        yield return ("Gemini client: count keeps the most recent candles, ascending", async () =>
        {
            var rows = Enumerable.Range(0, 5).Select(i => GeminiRow(1700000000 + i * 3600, 10, 11, 9, 10, 1));
            var h = new FakeHandler((_, _) => FakeHandler.Json(HttpStatusCode.OK, "[" + string.Join(",", rows.Reverse()) + "]"));
            var r = await NewClient(h).GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 3);
            Check.Eq(3, r.Count);
            Check.Eq(1700000000L + 2 * 3600, r[0].Timestamp); Check.Eq(1700000000L + 4 * 3600, r[2].Timestamp);
        });
        yield return ("Gemini client: retries 429 and 5xx, then succeeds", async () =>
        {
            var h = new FakeHandler((_, n) => n switch
            {
                1 => FakeHandler.Json((HttpStatusCode)429, "{}"),
                2 => FakeHandler.Json(HttpStatusCode.BadGateway, "oops"),
                _ => FakeHandler.Json(HttpStatusCode.OK, "[" + GeminiRow(1700000000, 1, 2, 1, 2, 5) + "]"),
            });
            var r = await NewClient(h).GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 5);
            Check.Eq(3, h.Requests.Count); Check.Eq(1, r.Count);
        });
        yield return ("Gemini client: 4xx is not retried; gives up after MaxAttempts on 5xx", async () =>
        {
            var bad = new FakeHandler((_, _) => FakeHandler.Json(HttpStatusCode.BadRequest, "{\"reason\":\"InvalidSymbol\"}"));
            var ex = await Capture<MarketDataException>(() => NewClient(bad).GetRecentCandlesAsync("XXXYYY", CandleInterval.Hour1, 5));
            Check.Eq(400, ex.StatusCode!.Value); Check.Eq(1, bad.Requests.Count);
            Check.True(ex.Message.Contains("InvalidSymbol", StringComparison.Ordinal));

            var down = new FakeHandler((_, _) => FakeHandler.Json(HttpStatusCode.ServiceUnavailable, "down"));
            await Check.ThrowsAsync<MarketDataException>(() => NewClient(down).GetRecentCandlesAsync("BTCUSD", CandleInterval.Hour1, 5));
            Check.Eq(3, down.Requests.Count, "MaxAttempts");
        });

        // ---- CSV / CLI --------------------------------------------------
        yield return ("CSV: round-trip, ISO timestamps, header and comments", () => Sync(() =>
        {
            var src = FromCloses(new decimal[] { 10.5m, 11.25m, 9.75m });
            var sw = new StringWriter(); CsvCandles.Write(sw, src);
            var back = CsvCandles.Read(new StringReader(sw.ToString()));
            Check.Eq(src.Count, back.Count);
            for (var i = 0; i < src.Count; i++) Check.Eq(src[i], back[i]);

            var iso = CsvCandles.Read(new StringReader("# c\ntime,o,h,l,c\n2024-01-01T00:00:00Z,1,2,1,2\n2024-01-01T01:00:00Z,2,3,2,3\n"));
            Check.Eq(1704067200L, iso[0].Timestamp);
            Check.Throws<FormatException>(() => CsvCandles.Read(new StringReader("1,2,3")));
            Check.Throws<ArgumentException>(() => CsvCandles.Read(new StringReader("2,1,1,1,1\n1,1,1,1,1")));
        }));
        yield return ("CLI: parser rejects unknown options and stray arguments", () => Sync(() =>
        {
            var cl = CommandLine.Parse(new[] { "backtest", "--fast", "5", "--bogus", "1" });
            Check.Eq("backtest", cl.Command); Check.Eq(5, cl.GetInt("fast", 0));
            Check.Throws<UsageException>(cl.EnsureNoUnknownOptions);
            Check.Throws<UsageException>(() => CommandLine.Parse(new[] { "a", "b" }));
            Check.Throws<UsageException>(() => CommandLine.Parse(new[] { "x", "--n", "abc" }).GetInt("n", 0));
        }));
        yield return ("CLI: backtest command end-to-end on CSV (offline)", async () =>
        {
            var closes = Enumerable.Range(0, 60).Select(i => 100m + (decimal)Math.Sin(i / 4.0) * 20m);
            var path = Path.Combine(Path.GetTempPath(), $"ztrade-{Guid.NewGuid():N}.csv");
            try
            {
                await using (var w = new StreamWriter(path)) CsvCandles.Write(w, FromCloses(closes));
                var output = new StringWriter();
                var cmds = new Commands(new ThrowingMarket(), output);
                await cmds.RunAsync(CommandLine.Parse(new[] { "backtest", "--csv", path, "--fast", "3", "--slow", "9" }), CancellationToken.None);
                Check.True(output.ToString().Contains("buy & hold", StringComparison.Ordinal));
                var patterns = new StringWriter();
                await new Commands(new ThrowingMarket(), patterns).RunAsync(CommandLine.Parse(new[] { "patterns", "--csv", path }), CancellationToken.None);
                Check.True(patterns.ToString().Contains("candles scanned", StringComparison.Ordinal));
            }
            finally
            {
                File.Delete(path);
            }
        });
    }

    private static GeminiMarketDataClient NewClient(FakeHandler handler, DateTimeOffset? now = null, bool includeUnclosed = false) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://api.gemini.com/") },
            new GeminiClientOptions { MaxAttempts = 3, BaseRetryDelay = TimeSpan.Zero, IncludeUnclosedCandles = includeUnclosed },
            now is null ? null : new FixedTimeProvider(now.Value));

    private static Task Sync(Action action)
    {
        action();
        return Task.CompletedTask;
    }

    private static async Task<TException> Capture<TException>(Func<Task> action) where TException : Exception
    {
        try { await action(); }
        catch (TException ex) { return ex; }
        throw new AssertionException($"expected {typeof(TException).Name}");
    }

    private sealed class ThrowingMarket : IMarketDataClient
    {
        public Task<IReadOnlyList<Candle>> GetRecentCandlesAsync(string symbol, CandleInterval interval, int count, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("network must not be used in offline mode");
    }
}
