using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Handler for UpdateTaskCommand
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _context;

    public UpdateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FindAsync([request.Id], cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return Result.NotFound<TaskDto>("Task not found", "TASK_NOT_FOUND");
        }

        // Check if category exists
        var category = await _context.Categories.FindAsync([request.CategoryId], cancellationToken).ConfigureAwait(false);
        if (category is null)
        {
            return Result.BadRequest<TaskDto>("Category not found", "CATEGORY_NOT_FOUND");
        }

        task.UpdateFromDto(request);
        task.UpdatedAtUtc = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Load the task with category for DTO mapping
        var updatedTask = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken).ConfigureAwait(false);

        return Result.Ok(updatedTask!.ToDto(), "Task updated successfully");
    }
}
