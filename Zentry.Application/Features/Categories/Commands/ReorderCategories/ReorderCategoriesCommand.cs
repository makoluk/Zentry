using MediatR;
using Zentry.Application.Common;

namespace Zentry.Application.Features.Categories.Commands.ReorderCategories;

/// <summary>
/// Command to reorder categories
/// </summary>
public record ReorderCategoriesCommand : IRequest<Result>
{
    public List<CategoryOrderItem> Categories { get; init; } = [];
}

/// <summary>
/// Category order item for reordering
/// </summary>
public record CategoryOrderItem
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
}
