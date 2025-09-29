using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Categories.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category
/// </summary>
public record UpdateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}
