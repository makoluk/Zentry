using Microsoft.AspNetCore.Mvc;
using Zentry.Api.Models;
using Zentry.Application.Common;
using ProblemDetails = Zentry.Api.Models.ProblemDetails;

namespace Zentry.Api.Extensions;

/// <summary>
/// Extension methods for converting Result to HTTP responses
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an appropriate HTTP response
    /// </summary>
    public static ActionResult FromResult(this ControllerBase controller, Result result)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);
        
        if (result.Success)
        {
            return result.Status switch
            {
                200 => new OkObjectResult(ApiResponse.SuccessResponse(message: result.Message)),
                201 => new CreatedResult(string.Empty, ApiResponse.SuccessResponse(message: result.Message)),
                204 => new NoContentResult(),
                _ => new OkObjectResult(ApiResponse.SuccessResponse(message: result.Message))
            };
        }

        var errorResponse = ApiResponse.ErrorResponse(
            result.Message ?? GetTitleForStatus(result.Status),
            result.ErrorCode != null ? new { code = result.ErrorCode } : null,
            controller.HttpContext.TraceIdentifier
        );

        return result.Status switch
        {
            400 => new BadRequestObjectResult(errorResponse),
            404 => new NotFoundObjectResult(errorResponse),
            422 => new UnprocessableEntityObjectResult(errorResponse),
            500 => new ObjectResult(errorResponse) { StatusCode = 500 },
            _ => new BadRequestObjectResult(errorResponse)
        };
    }

    /// <summary>
    /// Converts a Result<T> to an appropriate HTTP response
    /// </summary>
    public static ActionResult<T> FromResult<T>(this ControllerBase controller, Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);
        
        if (result.Success)
        {
            return result.Status switch
            {
                200 => new OkObjectResult(ApiResponse<T>.SuccessResponse(result.Data, result.Message)),
                201 => new CreatedResult(string.Empty, ApiResponse<T>.SuccessResponse(result.Data, result.Message)),
                _ => new OkObjectResult(ApiResponse<T>.SuccessResponse(result.Data, result.Message))
            };
        }

        var errorResponse = ApiResponse.ErrorResponse(
            result.Message ?? GetTitleForStatus(result.Status),
            result.ErrorCode != null ? new { code = result.ErrorCode } : null,
            controller.HttpContext.TraceIdentifier
        );

        return result.Status switch
        {
            400 => new BadRequestObjectResult(errorResponse),
            404 => new NotFoundObjectResult(errorResponse),
            422 => new UnprocessableEntityObjectResult(errorResponse),
            500 => new ObjectResult(errorResponse) { StatusCode = 500 },
            _ => new BadRequestObjectResult(errorResponse)
        };
    }

    private static string GetTitleForStatus(int status)
    {
        return status switch
        {
            400 => "Bad Request",
            404 => "Not Found",
            422 => "Unprocessable Entity",
            500 => "Internal Server Error",
            _ => "Error"
        };
    }
}