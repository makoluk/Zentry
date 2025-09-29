using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Zentry.Api.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
internal class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    private static readonly Action<ILogger, string, string, string, string, Exception?> LogRequestStarted =
        LoggerMessage.Define<string, string, string, string>(LogLevel.Information, new EventId(1, "RequestStarted"),
            "Request started: {Method} {Path} from {RemoteIp} | TraceId: {TraceId}");
    
    private static readonly Action<ILogger, int, long, string, Exception?> LogRequestCompleted =
        LoggerMessage.Define<int, long, string>(LogLevel.Information, new EventId(2, "RequestCompleted"),
            "Request completed: {StatusCode} in {ElapsedMs}ms | TraceId: {TraceId}");

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var stopwatch = Stopwatch.StartNew();
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log request
        LogRequestStarted(_logger,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            traceId,
            null);

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();

            // Log response
            LogRequestCompleted(_logger,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                traceId,
                null);

            // Restore response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream).ConfigureAwait(false);
        }
    }
}
