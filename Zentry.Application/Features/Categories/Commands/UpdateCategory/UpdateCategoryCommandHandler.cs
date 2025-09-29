using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Categories.Commands.UpdateCategory;

/// <summary>
/// Handler for UpdateCategoryCommand
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IAppDbContext _context;

    public UpdateCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([request.Id], cancellationToken).ConfigureAwait(false);
        if (category is null)
        {
            return Result.NotFound<CategoryDto>("Category not found", "CATEGORY_NOT_FOUND");
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.Color = request.Color;
        category.Icon = request.Icon;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAtUtc = DateTime.UtcNow;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(category.ToDto(), "Category updated successfully");
    }
}
