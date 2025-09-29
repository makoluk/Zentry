using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Categories.Commands.ReorderCategories;

/// <summary>
/// Handler for ReorderCategoriesCommand
/// </summary>
public class ReorderCategoriesCommandHandler : IRequestHandler<ReorderCategoriesCommand, Result>
{
    private readonly IAppDbContext _context;

    public ReorderCategoriesCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        if (request.Categories.Count == 0)
        {
            return Result.BadRequest("No categories provided for reordering", "NO_CATEGORIES");
        }

        // Get all category IDs from the request
        var categoryIds = request.Categories.Select(c => c.Id).ToList();

        // Verify all categories exist
        var existingCategories = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (existingCategories.Count != categoryIds.Count)
        {
            return Result.BadRequest("One or more categories not found", "CATEGORIES_NOT_FOUND");
        }

        // Update SortOrder for each category
        foreach (var orderItem in request.Categories)
        {
            var category = existingCategories.First(c => c.Id == orderItem.Id);
            category.SortOrder = orderItem.SortOrder;
            category.UpdatedAtUtc = DateTime.UtcNow;
        }

        _context.Categories.UpdateRange(existingCategories);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok("Categories reordered successfully");
    }
}
