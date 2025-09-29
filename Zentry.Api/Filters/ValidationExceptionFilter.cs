using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zentry.Api.Models;

namespace Zentry.Api.Filters;

/// <summary>
/// Action filter to handle FluentValidation exceptions
/// </summary>
internal sealed class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
                .ToArray();

            var response = ApiResponse.ErrorResponse(
                "One or more validation errors occurred",
                errors,
                context.HttpContext.TraceIdentifier
            );

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }
    }
}
