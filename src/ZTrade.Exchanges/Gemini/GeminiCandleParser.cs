using System.Globalization;
using System.Text.Json;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.Gemini;

public static class GeminiCandleParser
{
    public static IReadOnlyList<Candle> Parse(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new MarketDataException("Gemini returned invalid JSON.", inner: ex);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new MarketDataException("Gemini candle payload is not a JSON array.");
            }

            var byTimestamp = new SortedDictionary<long, Candle>();
            var rowNumber = 0;
            foreach (var row in document.RootElement.EnumerateArray())
            {
                rowNumber++;
                if (row.ValueKind != JsonValueKind.Array || row.GetArrayLength() < 6)
                {
                    throw new MarketDataException($"Row {rowNumber}: expected an array with at least 6 elements.");
                }

                try
                {
                    var milliseconds = long.Parse(Text(row[0]), NumberStyles.None, CultureInfo.InvariantCulture);
                    var candle = new Candle(
                        timestamp: milliseconds / 1000,
                        open: Dec(row[1]),
                        high: Dec(row[2]),
                        low: Dec(row[3]),
                        close: Dec(row[4]),
                        volume: Dec(row[5]));
                    byTimestamp[candle.Timestamp] = candle;
                }
                catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
                {
                    throw new MarketDataException($"Row {rowNumber}: invalid candle data ({ex.Message})", inner: ex);
                }
            }

            return byTimestamp.Values.ToArray();
        }
    }

    private static string Text(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString() ?? string.Empty,
        JsonValueKind.Number => element.GetRawText(),
        _ => string.Empty,
    };

    private static decimal Dec(JsonElement element) =>
        decimal.Parse(Text(element), NumberStyles.Float, CultureInfo.InvariantCulture);
}
