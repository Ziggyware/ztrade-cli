using ZTrade.Core.Indicators;
using ZTrade.Core.Market;

namespace ZTrade.Trading;

public enum Signal { Hold, Buy, Sell }

/// <summary>
/// Strategy contract: receives each CLOSED candle in ascending order and returns a signal for the NEXT bar.
/// Implementations must be deterministic and must not look ahead.
/// </summary>
public interface IStrategy
{
    string Name { get; }

    Signal OnCandle(in Candle candle);

    void Reset();
}

/// <summary>Buy when the fast SMA crosses above the slow SMA; sell on the opposite cross (cf. legacy MACross).</summary>
public sealed class SmaCrossStrategy : IStrategy
{
    private readonly Sma _fast;
    private readonly Sma _slow;
    private decimal? _previousDiff;

    public SmaCrossStrategy(int fastPeriod, int slowPeriod)
    {
        if (fastPeriod < 1 || slowPeriod <= fastPeriod)
        {
            throw new ArgumentException("Require 1 <= fastPeriod < slowPeriod.");
        }

        _fast = new Sma(fastPeriod);
        _slow = new Sma(slowPeriod);
        Name = $"SMA cross {fastPeriod}/{slowPeriod}";
    }

    public string Name { get; }

    public Signal OnCandle(in Candle candle)
    {
        var fast = _fast.Update(candle.Close);
        var slow = _slow.Update(candle.Close);
        if (fast is null || slow is null) return Signal.Hold;

        var diff = fast.Value - slow.Value;
        var previous = _previousDiff;
        _previousDiff = diff;
        if (previous is null) return Signal.Hold;

        if (previous <= 0m && diff > 0m) return Signal.Buy;
        if (previous >= 0m && diff < 0m) return Signal.Sell;
        return Signal.Hold;
    }

    public void Reset()
    {
        _fast.Reset();
        _slow.Reset();
        _previousDiff = null;
    }
}

/// <summary>Buy when RSI climbs back above the oversold level; sell when it falls back below overbought.</summary>
public sealed class RsiReversionStrategy : IStrategy
{
    private readonly Rsi _rsi;
    private readonly decimal _oversold;
    private readonly decimal _overbought;
    private decimal? _previous;

    public RsiReversionStrategy(int period = 14, decimal oversold = 30m, decimal overbought = 70m)
    {
        if (oversold <= 0m || overbought >= 100m || oversold >= overbought)
        {
            throw new ArgumentException("Require 0 < oversold < overbought < 100.");
        }

        _rsi = new Rsi(period);
        _oversold = oversold;
        _overbought = overbought;
        Name = $"RSI({period}) {oversold}/{overbought}";
    }

    public string Name { get; }

    public Signal OnCandle(in Candle candle)
    {
        var value = _rsi.Update(candle.Close);
        var previous = _previous;
        _previous = value;
        if (value is null || previous is null) return Signal.Hold;

        if (previous < _oversold && value >= _oversold) return Signal.Buy;
        if (previous >= _overbought && value < _overbought) return Signal.Sell;
        return Signal.Hold;
    }

    public void Reset()
    {
        _rsi.Reset();
        _previous = null;
    }
}
