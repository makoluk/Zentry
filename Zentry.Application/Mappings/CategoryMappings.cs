using Zentry.Application.DTOs;
using Zentry.Domain.Entities;

namespace Zentry.Application.Mappings;

/// <summary>
/// Mapping extensions for Category entity
/// </summary>
public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Color = entity.Color,
            Icon = entity.Icon,
            SortOrder = entity.SortOrder,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc,
            TaskCount = entity.Tasks.Count
        };
    }
}
