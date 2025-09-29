using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Categories.Commands.DeleteCategory;

/// <summary>
/// Handler for DeleteCategoryCommand
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IAppDbContext _context;

    public DeleteCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([request.Id], cancellationToken).ConfigureAwait(false);
        if (category is null)
        {
            return Result.NotFound("Category not found", "CATEGORY_NOT_FOUND");
        }

        // Check if category has tasks
        var hasTasks = await _context.Tasks.AnyAsync(t => t.CategoryId == request.Id, cancellationToken).ConfigureAwait(false);
        if (hasTasks)
        {
            return Result.BadRequest("Cannot delete category that has tasks", "CATEGORY_HAS_TASKS");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.NoContent();
    }
}
