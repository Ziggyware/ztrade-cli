using System.Globalization;
using ZTrade.Core.Market;

namespace ZTrade.Exchanges;

/// <summary>
/// Offline candle IO. Format: <c>timestamp,open,high,low,close[,volume]</c>; timestamp is Unix seconds or ISO-8601 (UTC).
/// An optional header row and lines starting with '#' are ignored.
/// </summary>
public static class CsvCandles
{
    public static IReadOnlyList<Candle> Read(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var candles = new List<Candle>();
        var lineNumber = 0;
        while (reader.ReadLine() is { } line)
        {
            lineNumber++;
            line = line.Trim();
            if (line.Length == 0 || line[0] == '#') continue;

            var cells = line.Split(',');
            if (cells.Length < 5) throw new FormatException($"Line {lineNumber}: expected at least 5 columns.");
            if (candles.Count == 0 && !char.IsDigit(cells[0].TrimStart()[0])) continue; // header

            try
            {
                candles.Add(new Candle(
                    ParseTimestamp(cells[0]),
                    Dec(cells[1]), Dec(cells[2]), Dec(cells[3]), Dec(cells[4]),
                    cells.Length > 5 && cells[5].Trim().Length > 0 ? Dec(cells[5]) : 0m));
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException or OverflowException)
            {
                throw new FormatException($"Line {lineNumber}: {ex.Message}", ex);
            }
        }

        CandleSeries.EnsureAscending(candles);
        return candles;
    }

    public static IReadOnlyList<Candle> ReadFile(string path)
    {
        using var reader = new StreamReader(path);
        return Read(reader);
    }

    public static void Write(TextWriter writer, IEnumerable<Candle> candles)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(candles);
        writer.Write("timestamp,open,high,low,close,volume\n");
        foreach (var c in candles)
        {
            writer.Write(string.Create(CultureInfo.InvariantCulture,
                $"{c.Timestamp},{c.Open},{c.High},{c.Low},{c.Close},{c.Volume}\n"));
        }
    }

    private static long ParseTimestamp(string text)
    {
        text = text.Trim();
        if (long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds)) return seconds;
        return DateTimeOffset.Parse(text, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToUnixTimeSeconds();
    }

    private static decimal Dec(string text) =>
        decimal.Parse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
}
