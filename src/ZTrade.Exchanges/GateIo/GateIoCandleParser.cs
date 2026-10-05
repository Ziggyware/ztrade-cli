using System.Globalization;
using System.Text.Json;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges.GateIo;

/// <summary>
/// Parses the Gate.io v4 <c>/spot/candlesticks</c> payload: an array of string arrays
/// <c>[unix_ts, quote_volume, close, high, low, open, base_volume, optional_window_closed]</c>.
/// </summary>
public static class GateIoCandleParser
{
    public static IReadOnlyList<Candle> Parse(string json, bool includeUnclosed = false)
    {
        ArgumentNullException.ThrowIfNull(json);

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new MarketDataException("Gate.io returned invalid JSON.", inner: ex);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new MarketDataException("Gate.io candlestick payload is not a JSON array.");
            }

            var candles = new List<Candle>();
            var rowNumber = 0;
            foreach (var row in document.RootElement.EnumerateArray())
            {
                rowNumber++;
                if (row.ValueKind != JsonValueKind.Array || row.GetArrayLength() < 6)
                {
                    throw new MarketDataException($"Row {rowNumber}: expected an array with at least 6 elements.");
                }

                var isClosed = row.GetArrayLength() < 8
                    || string.Equals(Text(row[7]), "true", StringComparison.OrdinalIgnoreCase);
                if (!isClosed && !includeUnclosed) continue;

                try
                {
                    candles.Add(new Candle(
                        timestamp: long.Parse(Text(row[0]), NumberStyles.None, CultureInfo.InvariantCulture),
                        open: Dec(row[5]),
                        high: Dec(row[3]),
                        low: Dec(row[4]),
                        close: Dec(row[2]),
                        // Gate.io field 1 is quote volume; field 6 is base amount and matches live trade size.
                        volume: Dec(row.GetArrayLength() >= 7 ? row[6] : row[1])));
                }
                catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
                {
                    throw new MarketDataException($"Row {rowNumber}: invalid candle data ({ex.Message})", inner: ex);
                }
            }

            candles.Sort(static (a, b) => a.Timestamp.CompareTo(b.Timestamp));
            return candles;
        }
    }

    private static string Text(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString() ?? string.Empty,
        JsonValueKind.Number => element.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        _ => string.Empty,
    };

    private static decimal Dec(JsonElement element) =>
        decimal.Parse(Text(element), NumberStyles.Float, CultureInfo.InvariantCulture);
}
