using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Handler for CreateCategoryCommand
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IAppDbContext _context;

    public CreateCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Get the next available SortOrder
        var maxSortOrder = 0;
        var existingCategories = await _context.Categories
            .Where(c => c.IsActive)
            .Select(c => c.SortOrder)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        
        if (existingCategories.Count > 0)
        {
            maxSortOrder = existingCategories.Max();
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            SortOrder = request.SortOrder > 0 ? request.SortOrder : maxSortOrder + 1,
            IsActive = request.IsActive
            // CreatedAtUtc ve UpdatedAtUtc otomatik setlenecek
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Created(category.ToDto(), "Category created successfully");
    }
}
