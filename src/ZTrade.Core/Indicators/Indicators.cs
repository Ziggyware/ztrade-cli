namespace ZTrade.Core.Indicators;

/// <summary>Incremental indicator: one implementation serves both batch and streaming use.</summary>
public interface IIndicator
{
    /// <summary>Feeds the next value; returns null until enough values have been seen (warm-up).</summary>
    decimal? Update(decimal value);

    void Reset();
}

/// <summary>Simple moving average. Running sum in <see cref="decimal"/> =&gt; no floating-point drift.</summary>
public sealed class Sma : IIndicator
{
    private readonly decimal[] _window;
    private int _count;
    private int _next;
    private decimal _sum;

    public Sma(int period)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        Period = period;
        _window = new decimal[period];
    }

    public int Period { get; }

    public decimal? Update(decimal value)
    {
        if (_count == Period) _sum -= _window[_next]; else _count++;
        _window[_next] = value;
        _sum += value;
        _next = (_next + 1) % Period;
        return _count == Period ? _sum / Period : null;
    }

    public void Reset()
    {
        Array.Clear(_window);
        _count = 0;
        _next = 0;
        _sum = 0m;
    }
}

/// <summary>Exponential moving average, seeded with the SMA of the first <c>period</c> values.</summary>
public sealed class Ema : IIndicator
{
    private readonly decimal _alpha;
    private readonly Sma _seed;
    private decimal? _value;

    public Ema(int period)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        Period = period;
        _alpha = 2m / (period + 1);
        _seed = new Sma(period);
    }

    public int Period { get; }

    public decimal? Update(decimal value)
    {
        if (_value is null)
        {
            _value = _seed.Update(value);
        }
        else
        {
            _value = _alpha * value + (1m - _alpha) * _value.Value;
        }

        return _value;
    }

    public void Reset()
    {
        _seed.Reset();
        _value = null;
    }
}

/// <summary>Wilder's RSI. First value after <c>period + 1</c> inputs (period price changes). Range 0..100.</summary>
public sealed class Rsi : IIndicator
{
    private decimal? _previous;
    private int _changes;
    private decimal _avgGain;
    private decimal _avgLoss;

    public Rsi(int period)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        Period = period;
    }

    public int Period { get; }

    public decimal? Update(decimal value)
    {
        if (_previous is null)
        {
            _previous = value;
            return null;
        }

        var change = value - _previous.Value;
        _previous = value;
        var gain = Math.Max(change, 0m);
        var loss = Math.Max(-change, 0m);

        if (_changes < Period)
        {
            _avgGain += gain;
            _avgLoss += loss;
            _changes++;
            if (_changes < Period) return null;
            _avgGain /= Period;
            _avgLoss /= Period;
        }
        else
        {
            _avgGain = (_avgGain * (Period - 1) + gain) / Period;
            _avgLoss = (_avgLoss * (Period - 1) + loss) / Period;
        }

        if (_avgLoss == 0m) return _avgGain == 0m ? 50m : 100m;
        var rs = _avgGain / _avgLoss;
        return 100m - 100m / (1m + rs);
    }

    public void Reset()
    {
        _previous = null;
        _changes = 0;
        _avgGain = 0m;
        _avgLoss = 0m;
    }
}

public static class IndicatorExtensions
{
    /// <summary>Runs an indicator over a series; output is index-aligned with the input (null during warm-up).</summary>
    public static decimal?[] Run(this IIndicator indicator, IReadOnlyList<decimal> values)
    {
        ArgumentNullException.ThrowIfNull(indicator);
        ArgumentNullException.ThrowIfNull(values);
        var result = new decimal?[values.Count];
        for (var i = 0; i < values.Count; i++) result[i] = indicator.Update(values[i]);
        return result;
    }
}
