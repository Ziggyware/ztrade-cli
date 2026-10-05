namespace ZTrade.Core.Market;

/// <summary>Bar width. Enum value == width in seconds.</summary>
public enum CandleInterval
{
    Second1 = 1,
    Second10 = 10,
    Minute1 = 60,
    Minute5 = 300,
    Minute15 = 900,
    Minute30 = 1800,
    Hour1 = 3600,
    Hour4 = 14400,
    Hour6 = 21600,
    Hour8 = 28800,
    Day1 = 86400,
    Day7 = 604800,
    Day30 = 2592000,
}

public static class CandleIntervalExtensions
{
    public static int Seconds(this CandleInterval interval) => (int)interval;

    public static TimeSpan ToTimeSpan(this CandleInterval interval) => TimeSpan.FromSeconds((int)interval);

    /// <summary>Short text form: 10s, 1m, 1hr, 1day, 7day, 30day.</summary>
    public static string ToShortString(this CandleInterval interval) => interval switch
    {
        CandleInterval.Second1 => "1s",
        CandleInterval.Second10 => "10s",
        CandleInterval.Minute1 => "1m",
        CandleInterval.Minute5 => "5m",
        CandleInterval.Minute15 => "15m",
        CandleInterval.Minute30 => "30m",
        CandleInterval.Hour1 => "1hr",
        CandleInterval.Hour4 => "4hr",
        CandleInterval.Hour6 => "6hr",
        CandleInterval.Hour8 => "8hr",
        CandleInterval.Day1 => "1day",
        CandleInterval.Day7 => "7day",
        CandleInterval.Day30 => "30day",
        _ => throw new ArgumentOutOfRangeException(nameof(interval)),
    };

    public static bool TryParse(string? text, out CandleInterval interval)
    {
        foreach (var candidate in Enum.GetValues<CandleInterval>())
        {
            if (string.Equals(candidate.ToShortString(), text, StringComparison.OrdinalIgnoreCase))
            {
                interval = candidate;
                return true;
            }
        }

        interval = default;
        return false;
    }
}
