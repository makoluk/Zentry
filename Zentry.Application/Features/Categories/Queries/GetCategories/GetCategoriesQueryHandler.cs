using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;

namespace Zentry.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// Handler for GetCategoriesQuery
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IAppDbContext _context;

    public GetCategoriesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Categories.AsQueryable();

        if (request.IsActive.HasValue)
        {
            queryable = queryable.Where(c => c.IsActive == request.IsActive.Value);
        }

        var categories = await queryable
            .Include(c => c.Tasks)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => c.ToDto())
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(categories);
    }
}
