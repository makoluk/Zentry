using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Categories.Queries.GetCategoryById;

/// <summary>
/// Query to get a category by ID
/// </summary>
public record GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; init; }
}
