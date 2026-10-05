using System.Globalization;
using System.Net;
using System.Text;
using ZTrade.Core.Market;
using ZTrade.Trading;

namespace ZTrade.Tests;

internal static class T
{
    public static Candle C(long ts, decimal o, decimal h, decimal l, decimal c, decimal v = 1m) => new(ts, o, h, l, c, v);

    /// <summary>Builds N candles with the given close path; open = previous close, tiny wicks.</summary>
    public static List<Candle> FromCloses(IEnumerable<decimal> closes, long startTs = 1_700_000_000, long step = 3600)
    {
        var list = new List<Candle>();
        decimal? prev = null;
        long ts = startTs;
        foreach (var close in closes)
        {
            var open = prev ?? close;
            list.Add(new Candle(ts, open, Math.Max(open, close) + 0.1m, Math.Min(open, close) - 0.1m, close, 100m));
            prev = close;
            ts += step;
        }

        return list;
    }

    public static string GeminiRow(long seconds, decimal o, decimal h, decimal l, decimal c, decimal v) =>
        string.Create(CultureInfo.InvariantCulture, $"[{seconds * 1000},{o},{h},{l},{c},{v}]");
}

internal sealed class ScriptedStrategy : IStrategy
{
    private readonly Dictionary<int, Signal> _script;
    private int _index = -1;

    public ScriptedStrategy(Dictionary<int, Signal> script) => _script = script;

    public string Name => "scripted";

    public Signal OnCandle(in Candle candle)
    {
        _index++;
        return _script.GetValueOrDefault(_index, Signal.Hold);
    }

    public void Reset() => _index = -1;
}

internal sealed class FakeHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, int, HttpResponseMessage> _respond;

    public FakeHandler(Func<HttpRequestMessage, int, HttpResponseMessage> respond) => _respond = respond;

    public List<Uri> Requests { get; } = new();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request.RequestUri!);
        return Task.FromResult(_respond(request, Requests.Count));
    }

    public static HttpResponseMessage Json(HttpStatusCode status, string body) =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
}

internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FixedTimeProvider(DateTimeOffset now) => _now = now;

    public override DateTimeOffset GetUtcNow() => _now;
}
