using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ZTrade.Core.Market;

namespace ZTrade.Cli;

internal sealed record ChartPattern(int Start, int End, string Type, string Bias);

internal static class ChartExporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Default,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task WriteAsync(
        string path,
        IReadOnlyList<Candle> candles,
        IReadOnlyList<ChartPattern> patterns,
        string symbol,
        string interval,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            symbol,
            interval,
            candles = candles.Select(c => new
            {
                time = c.Time.ToUniversalTime().ToString("O", System.Globalization.CultureInfo.InvariantCulture),
                open = c.Open,
                high = c.High,
                low = c.Low,
                close = c.Close,
                volume = c.Volume,
            }),
            patterns,
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var html = ChartPage.Html.Replace("__ZTRADE_DATA__", json, StringComparison.Ordinal);
        await File.WriteAllTextAsync(path, html, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), cancellationToken);
    }
}
