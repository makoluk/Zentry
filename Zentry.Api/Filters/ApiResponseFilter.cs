using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zentry.Api.Models;

namespace Zentry.Api.Filters;

/// <summary>
/// Automatically wraps controller responses in ApiResponse format
/// </summary>
internal class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // No action needed before execution
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            // Only wrap if it's not already an ApiResponse
            var valueType = objectResult.Value.GetType();
            if (valueType.Name != "ApiResponse" && !valueType.Name.StartsWith("ApiResponse`", StringComparison.Ordinal))
            {
                // Wrap the result in ApiResponse
                var apiResponse = ApiResponse<object>.SuccessResponse(
                    objectResult.Value,
                    GetSuccessMessage(context.ActionDescriptor.DisplayName ?? string.Empty)) with 
                { TraceId = context.HttpContext.TraceIdentifier };
                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }
        else if (context.Result is OkResult)
        {
            // Convert OkResult to ApiResponse
            var apiResponse = ApiResponse.SuccessResponse("Operation completed successfully") with 
            { TraceId = context.HttpContext.TraceIdentifier };
            context.Result = new ObjectResult(apiResponse) { StatusCode = 200 };
        }
    }

    private static string GetSuccessMessage(string actionName)
    {
        return actionName switch
        {
            var name when name.Contains("GetTasks", StringComparison.Ordinal) => "Tasks retrieved successfully",
            var name when name.Contains("GetTaskById", StringComparison.Ordinal) => "Task retrieved successfully",
            var name when name.Contains("CreateTask", StringComparison.Ordinal) => "Task created successfully",
            var name when name.Contains("UpdateTask", StringComparison.Ordinal) => "Task updated successfully",
            var name when name.Contains("ToggleTask", StringComparison.Ordinal) => "Task toggled successfully",
            var name when name.Contains("DeleteTask", StringComparison.Ordinal) => "Task deleted successfully",
            _ => "Operation completed successfully"
        };
    }
}
