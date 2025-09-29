using System.Net;
using System.Text.Json;
using FluentValidation;
using Zentry.Api.Models;

namespace Zentry.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    private static readonly Action<ILogger, string, Exception> LogUnhandledException =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, "UnhandledException"),
            "An unhandled exception occurred: {Message}");
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ValidationException ex)
        {
            LogUnhandledException(_logger, ex.Message, ex);
            await HandleValidationExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LogUnhandledException(_logger, ex.Message, ex);
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse.ErrorResponse(
            "An unexpected error occurred. Please try again later.",
            new { error = exception.Message },
            context.TraceIdentifier
        );

        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);

        await context.Response.WriteAsync(jsonResponse).ConfigureAwait(false);
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToArray();
        var response = ApiResponse.ErrorResponse(
            "One or more validation errors occurred",
            errors,
            context.TraceIdentifier
        );

        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);

        await context.Response.WriteAsync(jsonResponse).ConfigureAwait(false);
    }
}
