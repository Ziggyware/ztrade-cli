namespace ZTrade.Trading;

public enum Side { Buy, Sell }

public sealed record Fill(long Timestamp, Side Side, decimal Price, decimal Quantity, decimal Fee);

public sealed record Trade(
    long EntryTimestamp,
    long ExitTimestamp,
    decimal EntryPrice,
    decimal ExitPrice,
    decimal Quantity,
    decimal Pnl)
{
    public bool IsWin => Pnl > 0m;
}

/// <summary>
/// Simulated long-only spot account. Spends all cash on Buy, sells the whole position on Sell.
/// Fee and slippage are fractions (0.002 = 0.2%); slippage worsens each fill price.
/// This type NEVER talks to an exchange.
/// </summary>
public sealed class PaperBroker
{
    private readonly decimal _feeRate;
    private readonly decimal _slippageRate;
    private decimal _cashBeforeEntry;
    private Fill? _entry;

    public PaperBroker(decimal initialCash, decimal feeRate = 0.002m, decimal slippageRate = 0m)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(initialCash, 0m);
        if (feeRate is < 0m or >= 1m) throw new ArgumentOutOfRangeException(nameof(feeRate));
        if (slippageRate is < 0m or >= 1m) throw new ArgumentOutOfRangeException(nameof(slippageRate));
        InitialCash = initialCash;
        Cash = initialCash;
        _feeRate = feeRate;
        _slippageRate = slippageRate;
    }

    public decimal InitialCash { get; }
    public decimal Cash { get; private set; }
    public decimal Position { get; private set; }
    public bool IsInPosition => Position > 0m;
    public IReadOnlyList<Fill> Fills => _fills;
    public IReadOnlyList<Trade> Trades => _trades;

    private readonly List<Fill> _fills = new();
    private readonly List<Trade> _trades = new();

    public decimal Equity(decimal markPrice) => Cash + Position * markPrice;

    /// <summary>Buys with all cash at <paramref name="referencePrice"/> (+slippage). No-op if already in position.</summary>
    public Fill? Buy(long timestamp, decimal referencePrice)
    {
        if (IsInPosition || Cash <= 0m) return null;
        var price = referencePrice * (1m + _slippageRate);
        var quantity = Cash / (price * (1m + _feeRate));
        var fee = quantity * price * _feeRate;
        _cashBeforeEntry = Cash;
        Cash = 0m;
        Position = quantity;
        _entry = new Fill(timestamp, Side.Buy, price, quantity, fee);
        _fills.Add(_entry);
        return _entry;
    }

    /// <summary>Sells the whole position at <paramref name="referencePrice"/> (-slippage). No-op if flat.</summary>
    public Fill? Sell(long timestamp, decimal referencePrice)
    {
        if (!IsInPosition || _entry is null) return null;
        var price = referencePrice * (1m - _slippageRate);
        var quantity = Position;
        var proceeds = quantity * price;
        var fee = proceeds * _feeRate;
        Cash = proceeds - fee;
        Position = 0m;
        var fill = new Fill(timestamp, Side.Sell, price, quantity, fee);
        _fills.Add(fill);
        _trades.Add(new Trade(_entry.Timestamp, timestamp, _entry.Price, price, quantity, Cash - _cashBeforeEntry));
        _entry = null;
        return fill;
    }
}
