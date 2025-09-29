using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Tasks.Queries.GetTasks;

/// <summary>
/// Query to get paginated list of tasks
/// </summary>
public record GetTasksQuery : IRequest<Result<PagedResult<TaskDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool? IsDone { get; init; }
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
}
