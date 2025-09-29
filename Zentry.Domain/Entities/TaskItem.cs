using Zentry.Domain.Common;

namespace Zentry.Domain.Entities;

/// <summary>
/// TaskItem entity representing a task or work item
/// </summary>
public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    
    // Foreign key
    public Guid CategoryId { get; set; }
    
    // Navigation property
    public Category Category { get; set; } = null!;
}
