namespace ZTrade.Tests;

/// <summary>Dependency-free test harness (NuGet was unavailable when this was written). Swap for xunit when convenient.</summary>
public sealed class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}

public static class Check
{
    public static void True(bool condition, string message = "expected true")
    {
        if (!condition) throw new AssertionException(message);
    }

    public static void Eq<T>(T expected, T actual, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new AssertionException($"{message ?? "values differ"}: expected <{expected}>, actual <{actual}>");
        }
    }

    public static void Near(decimal expected, decimal actual, decimal tolerance, string? message = null)
    {
        if (Math.Abs(expected - actual) > tolerance)
        {
            throw new AssertionException($"{message ?? "not near"}: expected <{expected}> ± {tolerance}, actual <{actual}>");
        }
    }

    public static void Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        catch (Exception ex) { throw new AssertionException($"expected {typeof(TException).Name}, got {ex.GetType().Name}: {ex.Message}"); }
        throw new AssertionException($"expected {typeof(TException).Name}, nothing thrown");
    }

    public static async Task ThrowsAsync<TException>(Func<Task> action) where TException : Exception
    {
        try { await action(); }
        catch (TException) { return; }
        catch (Exception ex) { throw new AssertionException($"expected {typeof(TException).Name}, got {ex.GetType().Name}: {ex.Message}"); }
        throw new AssertionException($"expected {typeof(TException).Name}, nothing thrown");
    }
}
