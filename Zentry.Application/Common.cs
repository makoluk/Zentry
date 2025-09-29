namespace Zentry.Application.Common;

/// <summary>
/// Result pattern implementation for operation outcomes
/// </summary>
public class Result
{
    public bool Success { get; }
    public int Status { get; }
    public string? Message { get; }
    public string? ErrorCode { get; }

    protected Result(bool success, int status, string? message = null, string? errorCode = null)
    {
        Success = success;
        Status = status;
        Message = message;
        ErrorCode = errorCode;
    }

    public static Result Ok(string? message = null) => new(true, 200, message);
    public static Result Created(string? message = null) => new(true, 201, message);
    public static Result NoContent() => new(true, 204);
    
    public static Result BadRequest(string message, string? errorCode = null) => new(false, 400, message, errorCode);
    public static Result NotFound(string message, string? errorCode = null) => new(false, 404, message, errorCode);
    public static Result UnprocessableEntity(string message, string? errorCode = null) => new(false, 422, message, errorCode);
    public static Result InternalServerError(string message, string? errorCode = null) => new(false, 500, message, errorCode);

    public static Result<T> Ok<T>(T data, string? message = null) => new(data, true, 200, message);
    public static Result<T> Created<T>(T data, string? message = null) => new(data, true, 201, message);
    
    public static Result<T> BadRequest<T>(string message, string? errorCode = null) => new(default!, false, 400, message, errorCode);
    public static Result<T> NotFound<T>(string message, string? errorCode = null) => new(default!, false, 404, message, errorCode);
    public static Result<T> UnprocessableEntity<T>(string message, string? errorCode = null) => new(default!, false, 422, message, errorCode);
    public static Result<T> InternalServerError<T>(string message, string? errorCode = null) => new(default!, false, 500, message, errorCode);
}

/// <summary>
/// Generic result pattern implementation
/// </summary>
public class Result<T> : Result
{
    public T Data { get; }

    internal Result(T data, bool success, int status, string? message = null, string? errorCode = null) 
        : base(success, status, message, errorCode)
    {
        Data = data;
    }

    public static implicit operator Result<T>(T value) => Ok(value);
    
    public static Result<T> ToResult(T value) => Ok(value);
}

/// <summary>
/// Pagination query parameters
/// </summary>
public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}

/// <summary>
/// Paginated result wrapper
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public static PagedResult<T> Empty(int page = 1, int pageSize = 20) => 
        new(Array.Empty<T>(), 0, page, pageSize);
}

/// <summary>
/// Sorting options for queries
/// </summary>
public class SortingOptions
{
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}
