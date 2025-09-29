namespace Zentry.Application.DTOs;

/// <summary>
/// Task data transfer object for API responses
/// </summary>
public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    
    // Category information
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryColor { get; set; }
}

/// <summary>
/// DTO for creating a new task
/// </summary>
public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
}

/// <summary>
/// DTO for updating an existing task
/// </summary>
public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public Guid CategoryId { get; set; }
}
