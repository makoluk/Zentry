using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// Query to get all categories
/// </summary>
public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
    public bool? IsActive { get; init; }
}
