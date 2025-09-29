namespace Zentry.Api.Models;

/// <summary>
/// Standard API response wrapper
/// </summary>
public record ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public object? Data { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse SuccessResponse(object? data = null, string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse ErrorResponse(string message, object? errors = null, string? traceId = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = errors,
            TraceId = traceId
        };
    }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
public record ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> SuccessResponse(T? data = default, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
}

/// <summary>
/// Problem Details for error responses (RFC7807)
/// </summary>
public record ProblemDetails
{
    public string Type { get; init; } = "about:blank";
    public string Title { get; init; } = string.Empty;
    public int Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
    public string? TraceId { get; init; }
    public string? Code { get; init; }
    public Dictionary<string, object> Extensions { get; init; } = new();
}