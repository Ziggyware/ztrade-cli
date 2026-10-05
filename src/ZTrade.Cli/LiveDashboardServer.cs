using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZTrade.Cli;

/// <summary>Small local HTTP + Server-Sent Events host; no cloud service, CDN, or browser-side exchange key.</summary>
public sealed class LiveDashboardServer : IAsyncDisposable
{
    private const int MaxConcurrentClients = 32;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.Strict,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    private readonly HttpListener _listener = new();
    private readonly LiveMarketState _state;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly SemaphoreSlim _clientSlots = new(MaxConcurrentClients, MaxConcurrentClients);
    private Task? _acceptTask;
    private bool _disposed;

    public LiveDashboardServer(LiveMarketState state, string bindAddress, int port)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        ArgumentException.ThrowIfNullOrWhiteSpace(bindAddress);
        ArgumentOutOfRangeException.ThrowIfLessThan(port, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65_535);

        var address = IPAddress.TryParse(bindAddress, out var parsed) ? parsed :
            throw new ArgumentException("Bind address must be an IP address such as 127.0.0.1 or 0.0.0.0.", nameof(bindAddress));
        var prefixHost = IPAddress.Any.Equals(address) || IPAddress.IPv6Any.Equals(address)
            ? "+"
            : address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                ? $"[{address}]"
                : address.ToString();
        _listener.Prefixes.Add($"http://{prefixHost}:{port}/");

        var browserHost = IPAddress.Any.Equals(address) || IPAddress.IPv6Any.Equals(address)
            ? "127.0.0.1"
            : prefixHost;
        LocalUri = new Uri($"http://{browserHost}:{port}/", UriKind.Absolute);
        BindAddress = address.ToString();
        Port = port;
    }

    public string BindAddress { get; }
    public int Port { get; }
    public Uri LocalUri { get; }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_listener.IsListening) throw new InvalidOperationException("The dashboard server is already running.");
        _listener.Start();
        _acceptTask = AcceptLoopAsync(_shutdown.Token);
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
                if (!_clientSlots.Wait(0))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    context.Response.Close();
                    continue;
                }

                _ = HandleClientAsync(context, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (HttpListenerException) when (cancellationToken.IsCancellationRequested || !_listener.IsListening)
        {
        }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested || _disposed)
        {
        }
    }

    private async Task HandleClientAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers[HttpResponseHeader.Allow] = "GET";
                await WriteTextAsync(context.Response, HttpStatusCode.MethodNotAllowed, "GET only.", "text/plain; charset=utf-8", cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            var path = context.Request.Url?.AbsolutePath ?? "/";
            switch (path)
            {
                case "/":
                    context.Response.Headers["Content-Security-Policy"] =
                        "default-src 'self'; script-src 'unsafe-inline'; style-src 'unsafe-inline'; connect-src 'self'; img-src 'self' data:; base-uri 'none'; frame-ancestors 'none'";
                    await WriteTextAsync(context.Response, HttpStatusCode.OK, LiveDashboardPage.Html,
                        "text/html; charset=utf-8", cancellationToken).ConfigureAwait(false);
                    break;

                case "/api/snapshot":
                    await WriteTextAsync(context.Response, HttpStatusCode.OK,
                        JsonSerializer.Serialize(_state.Snapshot(), JsonOptions),
                        "application/json; charset=utf-8", cancellationToken).ConfigureAwait(false);
                    break;

                case "/healthz":
                    var snapshot = _state.Snapshot();
                    await WriteTextAsync(context.Response, HttpStatusCode.OK,
                        JsonSerializer.Serialize(new { status = snapshot.ConnectionState, exchange = snapshot.Exchange, symbol = snapshot.Symbol }, JsonOptions),
                        "application/json; charset=utf-8", cancellationToken).ConfigureAwait(false);
                    break;

                case "/events":
                    await WriteEventsAsync(context, cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    await WriteTextAsync(context.Response, HttpStatusCode.NotFound, "Not found.", "text/plain; charset=utf-8", cancellationToken)
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex) when (ex is IOException or HttpListenerException or ObjectDisposedException
                                       or OperationCanceledException)
        {
            // The browser navigated away or the user stopped the server. No retry/log loop is needed.
        }
        finally
        {
            try { context.Response.Close(); }
            catch (HttpListenerException) { }
            catch (ObjectDisposedException) { }
            _clientSlots.Release();
        }
    }

    private async Task WriteEventsAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        using var subscription = _state.Subscribe();
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/event-stream; charset=utf-8";
        response.SendChunked = true;
        response.KeepAlive = true;
        response.Headers[HttpResponseHeader.CacheControl] = "no-cache, no-store, must-revalidate";
        response.Headers["X-Accel-Buffering"] = "no";
        await WriteEventAsync(response, "snapshot", subscription.InitialSnapshot, cancellationToken).ConfigureAwait(false);

        using var heartbeat = new PeriodicTimer(TimeSpan.FromSeconds(15));
        var availableTask = subscription.Reader.WaitToReadAsync(cancellationToken).AsTask();
        var heartbeatTask = heartbeat.WaitForNextTickAsync(cancellationToken).AsTask();
        while (!cancellationToken.IsCancellationRequested)
        {
            var completed = await Task.WhenAny(availableTask, heartbeatTask).ConfigureAwait(false);
            if (completed == heartbeatTask)
            {
                if (!await heartbeatTask.ConfigureAwait(false)) break;
                await WriteRawAsync(response, ": keep-alive\n\n", cancellationToken).ConfigureAwait(false);
                heartbeatTask = heartbeat.WaitForNextTickAsync(cancellationToken).AsTask();
            }

            if (completed == availableTask)
            {
                if (!await availableTask.ConfigureAwait(false)) break;
                while (subscription.Reader.TryRead(out var update))
                {
                    await WriteEventAsync(response, "update", update, cancellationToken).ConfigureAwait(false);
                }

                availableTask = subscription.Reader.WaitToReadAsync(cancellationToken).AsTask();
            }
        }
    }

    private static async Task WriteEventAsync(
        HttpListenerResponse response,
        string eventName,
        LiveMarketSnapshot update,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(update, JsonOptions);
        await WriteRawAsync(response, $"event: {eventName}\ndata: {json}\n\n", cancellationToken).ConfigureAwait(false);
    }

    private static async Task WriteRawAsync(HttpListenerResponse response, string value, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        await response.OutputStream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await response.OutputStream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task WriteTextAsync(
        HttpListenerResponse response,
        HttpStatusCode status,
        string body,
        string contentType,
        CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        response.StatusCode = (int)status;
        response.ContentType = contentType;
        response.ContentLength64 = bytes.LongLength;
        response.Headers[HttpResponseHeader.CacheControl] = "no-cache, no-store, must-revalidate";
        response.Headers["X-Content-Type-Options"] = "nosniff";
        await response.OutputStream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _shutdown.Cancel();
        _listener.Close();
        if (_acceptTask is not null)
        {
            try { await _acceptTask.ConfigureAwait(false); }
            catch (HttpListenerException) { }
        }

        _shutdown.Dispose();
    }
}
