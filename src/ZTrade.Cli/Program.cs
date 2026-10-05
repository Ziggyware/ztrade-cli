using ZTrade.Cli;
using ZTrade.Exchanges.GateIo;
using ZTrade.Exchanges.Gemini;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var handler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) };
using var http = new HttpClient(handler, disposeHandler: false)
{
    BaseAddress = new Uri("https://api.gemini.com/"),
    Timeout = TimeSpan.FromSeconds(30),
};
http.DefaultRequestHeaders.UserAgent.ParseAdd("ztrade-cli/1.0");
http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

using var gateHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) };
using var gateHttp = new HttpClient(gateHandler, disposeHandler: false)
{
    BaseAddress = new Uri("https://api.gateio.ws/api/v4/"),
    Timeout = TimeSpan.FromSeconds(30),
};
gateHttp.DefaultRequestHeaders.UserAgent.ParseAdd("ztrade-cli/1.0");
gateHttp.DefaultRequestHeaders.Accept.ParseAdd("application/json");

var commands = new Commands(new GeminiMarketDataClient(http), Console.Out, new GateIoMarketDataClient(gateHttp));

try
{
    var commandLine = CommandLine.Parse(args);
    if (commandLine.Command is null or "help" || args.Contains("--help"))
    {
        Console.Out.WriteLine(Commands.Usage);
        return commandLine.Command is null ? 2 : 0;
    }

    await commands.RunAsync(commandLine, cts.Token);
    return 0;
}
catch (UsageException ex)
{
    await Console.Error.WriteLineAsync($"error: {ex.Message}\n");
    await Console.Error.WriteLineAsync(Commands.Usage);
    return 2;
}
catch (OperationCanceledException)
{
    await Console.Error.WriteLineAsync("cancelled.");
    return 130;
}
catch (Exception ex) when (ex is ZTrade.Exchanges.MarketDataException or IOException or FormatException
                              or ArgumentException or NotSupportedException or System.Net.HttpListenerException)
{
    await Console.Error.WriteLineAsync($"error: {ex.Message}");
    return 1;
}

