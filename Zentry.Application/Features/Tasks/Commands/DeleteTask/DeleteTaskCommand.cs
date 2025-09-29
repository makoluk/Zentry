using MediatR;
using Zentry.Application.Common;

namespace Zentry.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Command to delete a task
/// </summary>
public record DeleteTaskCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
