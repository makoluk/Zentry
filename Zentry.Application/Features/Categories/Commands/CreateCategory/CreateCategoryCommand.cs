using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Command to create a new category
/// </summary>
public record CreateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;
}
