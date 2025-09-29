using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Tasks.Queries.GetTaskById;

/// <summary>
/// Query to get a task by ID
/// </summary>
public record GetTaskByIdQuery : IRequest<Result<TaskDto>>
{
    public Guid Id { get; init; }
}
