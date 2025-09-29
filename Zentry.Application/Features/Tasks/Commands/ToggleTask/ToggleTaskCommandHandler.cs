using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Commands.ToggleTask;

/// <summary>
/// Handler for ToggleTaskCommand
/// </summary>
public class ToggleTaskCommandHandler : IRequestHandler<ToggleTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _context;

    public ToggleTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskDto>> Handle(ToggleTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return Result.NotFound<TaskDto>("Task not found", "TASK_NOT_FOUND");
        }

        task.IsDone = !task.IsDone;
        task.UpdatedAtUtc = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(task.ToDto(), "Task status toggled successfully");
    }
}
