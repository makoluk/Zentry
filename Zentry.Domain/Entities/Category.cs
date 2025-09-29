using Zentry.Domain.Common;

namespace Zentry.Domain.Entities;

/// <summary>
/// Category entity representing a task category
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; } // Hex color for UI
    public string? Icon { get; set; } // Icon name for UI
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
