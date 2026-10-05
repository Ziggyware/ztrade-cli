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

// ---------------------------------------------------------------------------
// 10 ultra indicative indicators — OHLCV-derived, institutional-grade lens
// ---------------------------------------------------------------------------

/// <summary>Ultra indicative: Weighted Moving Average — linear weighting emphasizes recent price.</summary>
public sealed class Wma : IIndicator
{
    private readonly int _period;
    private readonly Queue<decimal> _window = new();

    public Wma(int period)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period;
    }

    public int Period => _period;

    public decimal? Update(decimal value)
    {
        _window.Enqueue(value);
        if (_window.Count > _period) _window.Dequeue();
        if (_window.Count < _period) return null;
        var arr = _window.ToArray();
        decimal weighted = 0m; decimal denom = 0m;
        for (var i = 0; i < arr.Length; i++) { var w = i + 1; weighted += arr[i] * w; denom += w; }
        return weighted / denom;
    }

    public void Reset() => _window.Clear();
}

/// <summary>Ultra indicative: Hull Moving Average — ultra-responsive yet smooth trend filter.</summary>
public sealed class HullMovingAverage : IIndicator
{
    private readonly Wma _wmaHalf;
    private readonly Wma _wmaFull;
    private readonly int _sqrtN;
    private readonly Queue<decimal> _raw = new();

    public HullMovingAverage(int period = 21)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _wmaHalf = new Wma(period / 2 == 0 ? 1 : period / 2);
        _wmaFull = new Wma(period);
        _sqrtN = Math.Max(1, (int)Math.Sqrt(period));
    }

    public decimal? Update(decimal value)
    {
        var a = _wmaHalf.Update(value);
        var b = _wmaFull.Update(value);
        if (a is null || b is null) return null;
        var diff = 2m * a.Value - b.Value;
        _raw.Enqueue(diff);
        if (_raw.Count > _sqrtN) _raw.Dequeue();
        if (_raw.Count < _sqrtN) return null;
        var arr = _raw.ToArray();
        decimal weighted = 0m; decimal denom = 0m;
        for (var i = 0; i < arr.Length; i++) { var w = i + 1; weighted += arr[i] * w; denom += w; }
        return weighted / denom;
    }

    public void Reset() { _wmaHalf.Reset(); _wmaFull.Reset(); _raw.Clear(); }
}

/// <summary>Ultra indicative: Kaufman Adaptive Moving Average — adapts to noise vs trend efficiency.</summary>
public sealed class Kama : IIndicator
{
    private readonly int _period;
    private readonly int _fastN;
    private readonly int _slowN;
    private readonly Queue<decimal> _window = new();
    private decimal? _kama;

    public Kama(int period = 10, int fastPeriod = 2, int slowPeriod = 30)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period; _fastN = fastPeriod; _slowN = slowPeriod;
    }

    public decimal? Update(decimal value)
    {
        _window.Enqueue(value);
        if (_window.Count > _period + 1) _window.Dequeue();
        if (_window.Count <= _period) { if (_window.Count == _period + 1) _kama = value; return _kama; }
        var arr = _window.ToArray();
        decimal direction = Math.Abs(arr[^1] - arr[0]);
        decimal volatility = 0m;
        for (var i = 1; i < arr.Length; i++) volatility += Math.Abs(arr[i] - arr[i - 1]);
        var er = volatility == 0m ? 0m : direction / volatility;
        var fastAlpha = 2m / (_fastN + 1); var slowAlpha = 2m / (_slowN + 1);
        var sc = (er * (fastAlpha - slowAlpha) + slowAlpha);
        sc = sc * sc;
        _kama = _kama is null ? value : _kama.Value + sc * (value - _kama.Value);
        return _kama;
    }

    public void Reset() { _window.Clear(); _kama = null; }
}

/// <summary>Ultra indicative: Parabolic SAR — trailing stop &amp; trend flip detector.</summary>
public sealed class ParabolicSar : IIndicator
{
    private decimal? _sar;
    private decimal _ep;
    private decimal _af = 0.02m;
    private const decimal AfStep = 0.02m;
    private const decimal AfMax = 0.20m;
    private bool _isLong = true;
    private int _count;

    public decimal? Update(decimal value)
    {
        _count++;
        if (_count == 1) { _sar = value; _ep = value; return null; }
        if (_count == 2) { _isLong = value > _sar; _ep = value; _sar = _isLong ? Math.Min(_sar!.Value, value) : Math.Max(_sar!.Value, value); return _sar; }
        var sar = _sar!.Value;
        if (_isLong)
        {
            if (value < sar) { _isLong = false; _sar = _ep; _ep = value; _af = 0.02m; return _sar; }
            if (value > _ep) { _ep = value; _af = Math.Min(AfMax, _af + AfStep); }
            _sar = sar + _af * (_ep - sar);
        }
        else
        {
            if (value > sar) { _isLong = true; _sar = _ep; _ep = value; _af = 0.02m; return _sar; }
            if (value < _ep) { _ep = value; _af = Math.Min(AfMax, _af + AfStep); }
            _sar = sar + _af * (_ep - sar);
        }
        return _sar;
    }

    public void Reset() { _sar = null; _ep = 0m; _af = 0.02m; _isLong = true; _count = 0; }
}

/// <summary>Ultra indicative: Williams %R — momentum overbought/oversold extreme detector.</summary>
public sealed class WilliamsR : IIndicator
{
    private readonly int _period;
    private readonly Queue<decimal> _closes = new();
    private readonly Queue<decimal> _highs = new();
    private readonly Queue<decimal> _lows = new();
    private decimal _lastClose;

    public WilliamsR(int period = 14)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period;
    }

    public decimal? Update(decimal value) => Update(value, value, value);

    public decimal? Update(decimal close, decimal high, decimal low)
    {
        _lastClose = close;
        _closes.Enqueue(close); _highs.Enqueue(high); _lows.Enqueue(low);
        if (_closes.Count > _period) { _closes.Dequeue(); _highs.Dequeue(); _lows.Dequeue(); }
        if (_closes.Count < _period) return null;
        var hh = _highs.Max(); var ll = _lows.Min();
        if (hh == ll) return -50m;
        return (hh - close) / (hh - ll) * -100m;
    }

    public void Reset() { _closes.Clear(); _highs.Clear(); _lows.Clear(); }
}

/// <summary>Ultra indicative: Rate of Change — pure momentum velocity.</summary>
public sealed class Roc : IIndicator
{
    private readonly int _period;
    private readonly Queue<decimal> _window = new();

    public Roc(int period = 12)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period;
    }

    public decimal? Update(decimal value)
    {
        _window.Enqueue(value);
        if (_window.Count > _period + 1) _window.Dequeue();
        if (_window.Count <= _period) return null;
        var arr = _window.ToArray();
        var prev = arr[0];
        if (prev == 0m) return 0m;
        return (value - prev) / Math.Abs(prev) * 100m;
    }

    public void Reset() => _window.Clear();
}

/// <summary>Ultra indicative: Aroon — time since highest high / lowest low, trend &amp; consolidation detector.</summary>
public sealed class Aroon
{
    private readonly int _period;
    private readonly Queue<decimal> _highs = new();
    private readonly Queue<decimal> _lows = new();

    public Aroon(int period = 14)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period;
    }

    public (decimal? Up, decimal? Down)? Update(decimal high, decimal low)
    {
        _highs.Enqueue(high); _lows.Enqueue(low);
        if (_highs.Count > _period + 1) { _highs.Dequeue(); _lows.Dequeue(); }
        if (_highs.Count <= _period) return null;
        var h = _highs.ToArray(); var l = _lows.ToArray();
        var hhIdx = Array.IndexOf(h, h.Max()); // last occurrence would be better but ok
        // find last occurrence of max
        for (var i = h.Length - 1; i >= 0; i--) if (h[i] == h.Max()) { hhIdx = i; break; }
        var llIdx = Array.IndexOf(l, l.Min());
        for (var i = l.Length - 1; i >= 0; i--) if (l[i] == l.Min()) { llIdx = i; break; }
        var sinceHigh = h.Length - 1 - hhIdx; var sinceLow = l.Length - 1 - llIdx;
        var up = (_period - sinceHigh) / (decimal)_period * 100m;
        var down = (_period - sinceLow) / (decimal)_period * 100m;
        return (up, down);
    }

    public void Reset() { _highs.Clear(); _lows.Clear(); }
}

/// <summary>Ultra indicative: Stochastic RSI — sensitivity inside RSI extremes.</summary>
public sealed class StochRsi : IIndicator
{
    private readonly Rsi _rsi;
    private readonly int _stochPeriod;
    private readonly Queue<decimal> _rsiWindow = new();

    public StochRsi(int rsiPeriod = 14, int stochPeriod = 14)
    {
        _rsi = new Rsi(rsiPeriod);
        _stochPeriod = stochPeriod;
    }

    public decimal? Update(decimal value)
    {
        var r = _rsi.Update(value);
        if (r is null) return null;
        _rsiWindow.Enqueue(r.Value);
        if (_rsiWindow.Count > _stochPeriod) _rsiWindow.Dequeue();
        if (_rsiWindow.Count < _stochPeriod) return null;
        var arr = _rsiWindow.ToArray();
        var mn = arr.Min(); var mx = arr.Max();
        if (mx == mn) return 50m;
        return (r.Value - mn) / (mx - mn) * 100m;
    }

    public void Reset() { _rsi.Reset(); _rsiWindow.Clear(); }
}

/// <summary>Ultra indicative: Bollinger Bandwidth — volatility squeeze &amp; expansion detector.</summary>
public sealed class BollingerBandwidth : IIndicator
{
    private readonly int _period;
    private readonly Queue<decimal> _window = new();

    public BollingerBandwidth(int period = 20)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(period, 1);
        _period = period;
    }

    public decimal? Update(decimal value)
    {
        _window.Enqueue(value);
        if (_window.Count > _period) _window.Dequeue();
        if (_window.Count < _period) return null;
        var arr = _window.ToArray();
        var sma = arr.Average();
        decimal variance = 0m;
        foreach (var v in arr) variance += (v - sma) * (v - sma);
        var std = (decimal)Math.Sqrt((double)(variance / _period));
        var upper = sma + 2m * std; var lower = sma - 2m * std;
        if (sma == 0m) return null;
        return (upper - lower) / sma * 100m;
    }

    public void Reset() => _window.Clear();
}

/// <summary>Ultra indicative: Coppock Curve — long-term momentum bottom detector (institutional accumulation).</summary>
public sealed class CoppockCurve : IIndicator
{
    private readonly Roc _roc11 = new(11);
    private readonly Roc _roc14 = new(14);
    private readonly Wma _wma10 = new(10);
    private readonly Queue<decimal> _rocSum = new();

    public decimal? Update(decimal value)
    {
        var r11 = _roc11.Update(value);
        var r14 = _roc14.Update(value);
        if (r11 is null || r14 is null) return null;
        var sum = r11.Value + r14.Value;
        return _wma10.Update(sum);
    }

    public void Reset() { _roc11.Reset(); _roc14.Reset(); _wma10.Reset(); }
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
