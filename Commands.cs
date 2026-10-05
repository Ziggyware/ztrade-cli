using System.Globalization;
using ZTrade.Core.Market;
using ZTrade.Core.Patterns;
using ZTrade.Exchanges;
using ZTrade.Exchanges.Gemini;
using ZTrade.Trading;

namespace ZTrade.Cli;

public sealed class Commands
{
    private readonly IMarketDataClient _market;
    private readonly TextWriter _out;

    public Commands(IMarketDataClient market, TextWriter output)
    {
        _market = market;
        _out = output;
    }

    public const string Usage = """
        ztrade — market data, pattern scanning and paper backtesting (no live trading).

        Usage:
          ztrade candles  --symbol BTCUSD --interval 1hr [--count 50] [--out file.csv]
          ztrade patterns (--symbol BTCUSD --interval 1hr [--count 300] | --csv file.csv)
          ztrade backtest (--symbol BTCUSD --interval 1hr [--count 1000] | --csv file.csv)
                          [--strategy sma|rsi] [--fast 10] [--slow 30]
                          [--period 14] [--oversold 30] [--overbought 70]
                          [--cash 10000] [--fee 0.002] [--slippage 0]

        Exchange intervals: 1m 5m 15m 30m 1hr 6hr 1day
        """;

    public async Task RunAsync(CommandLine cl, CancellationToken ct)
    {
        switch (cl.Command?.ToLowerInvariant())
        {
            case "candles": await CandlesAsync(cl, ct); break;
            case "patterns": await PatternsAsync(cl, ct); break;
            case "backtest": await BacktestAsync(cl, ct); break;
            default: throw new UsageException(cl.Command is null ? "No command given." : $"Unknown command '{cl.Command}'.");
        }
    }

    private async Task<IReadOnlyList<Candle>> LoadAsync(CommandLine cl, int defaultCount, CancellationToken ct)
    {
        if (cl.Get("csv") is { } path)
        {
            _ = cl.Get("symbol"); _ = cl.Get("interval"); _ = cl.Get("count");
            return CsvCandles.ReadFile(path);
        }

        var symbol = cl.Require("symbol");
        var intervalText = cl.Require("interval");
        if (!CandleIntervalExtensions.TryParse(intervalText, out var interval))
        {
            throw new UsageException($"Unknown interval '{intervalText}'.");
        }

        return await _market.GetRecentCandlesAsync(symbol, interval, cl.GetInt("count", defaultCount), ct);
    }

    private async Task CandlesAsync(CommandLine cl, CancellationToken ct)
    {
        var outPath = cl.Get("out");
        var candles = await LoadAsync(cl, 50, ct);
        cl.EnsureNoUnknownOptions();

        if (outPath is not null)
        {
            await using var writer = new StreamWriter(outPath);
            CsvCandles.Write(writer, candles);
            _out.WriteLine($"Wrote {candles.Count} candles to {outPath}");
            return;
        }

        _out.WriteLine($"{"Time (UTC)",-17} {"Open",14} {"High",14} {"Low",14} {"Close",14} {"Volume",16}");
        foreach (var c in candles)
        {
            _out.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"{c.Time:yyyy-MM-dd HH:mm}  {c.Open,14} {c.High,14} {c.Low,14} {c.Close,14} {c.Volume,16:N2}"));
        }
    }

    private async Task PatternsAsync(CommandLine cl, CancellationToken ct)
    {
        var candles = await LoadAsync(cl, 300, ct);
        cl.EnsureNoUnknownOptions();

        var matches = PatternScanner.Scan(candles);
        _out.WriteLine($"{candles.Count} candles scanned, {matches.Count} pattern matches.");
        foreach (var m in matches)
        {
            _out.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"{candles[m.EndIndex].Time:yyyy-MM-dd HH:mm}  {m.Type,-28} {m.Bias,-8} (candles {m.StartIndex}..{m.EndIndex})"));
        }

        _out.WriteLine();
        foreach (var group in matches.GroupBy(m => m.Type).OrderByDescending(g => g.Count()))
        {
            _out.WriteLine($"  {group.Key,-28} {group.Count(),5}");
        }
    }

    private async Task BacktestAsync(CommandLine cl, CancellationToken ct)
    {
        var strategyName = (cl.Get("strategy") ?? "sma").ToLowerInvariant();
        IStrategy strategy = strategyName switch
        {
            "sma" => new SmaCrossStrategy(cl.GetInt("fast", 10), cl.GetInt("slow", 30)),
            "rsi" => new RsiReversionStrategy(cl.GetInt("period", 14), cl.GetDecimal("oversold", 30m), cl.GetDecimal("overbought", 70m)),
            _ => throw new UsageException($"Unknown strategy '{strategyName}' (use sma or rsi)."),
        };
        var broker = new PaperBroker(cl.GetDecimal("cash", 10_000m), cl.GetDecimal("fee", 0.002m), cl.GetDecimal("slippage", 0m));
        var candles = await LoadAsync(cl, 1000, ct);
        cl.EnsureNoUnknownOptions();

        var r = BacktestEngine.Run(candles, strategy, broker);
        string Pct(decimal v) => (v * 100m).ToString("F2", CultureInfo.InvariantCulture) + "%";

        _out.WriteLine($"Strategy        {r.StrategyName}");
        _out.WriteLine($"Period          {candles[0].Time:yyyy-MM-dd} .. {candles[^1].Time:yyyy-MM-dd}  ({r.CandleCount} candles)");
        _out.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Equity          {r.InitialEquity:N2} -> {r.FinalEquity:N2}"));
        _out.WriteLine($"Return          {Pct(r.ReturnFraction)}   (buy & hold: {Pct(r.BuyAndHoldReturnFraction)})");
        _out.WriteLine($"Max drawdown    {Pct(r.MaxDrawdownFraction)}");
        _out.WriteLine($"Trades          {r.TradeCount}   wins: {r.WinCount}   win rate: {(r.WinRate is { } w ? Pct(w) : "n/a")}");
        _out.WriteLine($"Open position   {(r.EndedInPosition ? "yes (marked to market)" : "no")}");
        _out.WriteLine("Simulation only: fees/slippage are assumptions; no exchange orders were placed.");
    }
}
