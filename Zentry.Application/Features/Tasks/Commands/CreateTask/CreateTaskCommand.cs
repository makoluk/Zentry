using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Command to create a new task
/// </summary>
public record CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
}
