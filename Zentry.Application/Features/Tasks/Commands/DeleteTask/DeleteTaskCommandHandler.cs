using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IAppDbContext _context;

    public DeleteTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return Result.NotFound("Task not found", "TASK_NOT_FOUND");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.NoContent();
    }
}
