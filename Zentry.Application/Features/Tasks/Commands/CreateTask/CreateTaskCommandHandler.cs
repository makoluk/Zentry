using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Handler for CreateTaskCommand
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _context;

    public CreateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Check if category exists
        var category = await _context.Categories.FindAsync([request.CategoryId], cancellationToken).ConfigureAwait(false);
        if (category is null)
        {
            return Result.BadRequest<TaskDto>("Category not found", "CATEGORY_NOT_FOUND");
        }

        var task = request.ToEntity();
        // CreatedAtUtc ve UpdatedAtUtc otomatik setlenecek
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Load the task with category for DTO mapping
        var createdTask = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken).ConfigureAwait(false);

        return Result.Created(createdTask!.ToDto(), "Task created successfully");
    }
}
