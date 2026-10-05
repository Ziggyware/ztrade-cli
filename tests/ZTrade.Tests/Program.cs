using ZTrade.Tests;

var failures = 0;
var total = 0;
foreach (var (name, test) in AllTests.Collect())
{
    total++;
    try
    {
        await test();
        Console.WriteLine($"  PASS  {name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"  FAIL  {name}\n        {ex.GetType().Name}: {ex.Message}");
    }
}

Console.WriteLine($"\n{total - failures}/{total} passed.");
return failures == 0 ? 0 : 1;
