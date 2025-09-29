using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zentry.Api.Models;
using ProblemDetails = Zentry.Api.Models.ProblemDetails;

namespace Zentry.Api.Filters;

/// <summary>
/// Action filter for model validation
/// </summary>
public class ModelValidationFilter : IActionFilter
{
    private readonly ILogger<ModelValidationFilter> _logger;

    private static readonly Action<ILogger, string, Exception?> LogModelValidationFailed =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1, "ModelValidationFailed"),
            "Model validation failed: {Errors}");

    public ModelValidationFilter(ILogger<ModelValidationFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            LogModelValidationFailed(_logger, string.Join(", ", errors), null);

            var response = ApiResponse.ErrorResponse(
                "One or more validation errors occurred",
                errors.Select(e => new { field = "Model", message = e }).ToArray(),
                context.HttpContext.TraceIdentifier
            );

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
