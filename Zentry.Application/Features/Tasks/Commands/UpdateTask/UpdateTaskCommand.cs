using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Command to update an existing task
/// </summary>
public record UpdateTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDone { get; init; }
    public Guid CategoryId { get; init; }
}
