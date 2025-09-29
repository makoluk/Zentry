using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Zentry.Api.Filters;

/// <summary>
/// Action filter for logging controller actions
/// </summary>
public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;
    private readonly Stopwatch _stopwatch = new();

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    private static readonly Action<ILogger, string, string, string, Exception?> LogActionExecuting =
        LoggerMessage.Define<string, string, string>(LogLevel.Information, new EventId(1, "ActionExecuting"),
            "Action executing: {Controller}.{Action} | TraceId: {TraceId}");

    public void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _stopwatch.Start();
        
        var controllerName = context.Controller.GetType().Name;
        var actionName = context.ActionDescriptor.DisplayName ?? "Unknown";
        var traceId = context.HttpContext.TraceIdentifier;

        LogActionExecuting(_logger, controllerName, actionName, traceId, null);
    }

    private static readonly Action<ILogger, string, string, int, long, string, Exception?> LogActionCompleted =
        LoggerMessage.Define<string, string, int, long, string>(LogLevel.Information, new EventId(2, "ActionCompleted"),
            "Action completed: {Controller}.{Action} | Status: {StatusCode} | Duration: {Duration}ms | TraceId: {TraceId}");

    private static readonly Action<ILogger, string, string, int, long, string, Exception?> LogActionFailed =
        LoggerMessage.Define<string, string, int, long, string>(LogLevel.Error, new EventId(3, "ActionFailed"),
            "Action failed: {Controller}.{Action} | Status: {StatusCode} | Duration: {Duration}ms | TraceId: {TraceId}");

    public void OnActionExecuted(ActionExecutedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _stopwatch.Stop();
        
        var controllerName = context.Controller.GetType().Name;
        var actionName = context.ActionDescriptor.DisplayName ?? "Unknown";
        var traceId = context.HttpContext.TraceIdentifier;
        var statusCode = context.HttpContext.Response.StatusCode;
        var duration = _stopwatch.ElapsedMilliseconds;

        if (context.Exception != null)
        {
            LogActionFailed(_logger, controllerName, actionName, statusCode, duration, traceId, context.Exception);
        }
        else
        {
            LogActionCompleted(_logger, controllerName, actionName, statusCode, duration, traceId, null);
        }
    }
}
