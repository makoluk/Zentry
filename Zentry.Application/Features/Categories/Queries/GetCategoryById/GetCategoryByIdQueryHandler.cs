using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;

namespace Zentry.Application.Features.Categories.Queries.GetCategoryById;

/// <summary>
/// Handler for GetCategoryByIdQuery
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IAppDbContext _context;

    public GetCategoryByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken).ConfigureAwait(false);

        if (category is null)
        {
            return Result.NotFound<CategoryDto>("Category not found", "CATEGORY_NOT_FOUND");
        }

        return Result.Ok(category.ToDto());
    }
}
