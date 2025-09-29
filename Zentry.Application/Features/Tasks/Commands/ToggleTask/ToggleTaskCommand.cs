using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Tasks.Commands.ToggleTask;

/// <summary>
/// Command to toggle task completion status
/// </summary>
public record ToggleTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid Id { get; init; }
}
