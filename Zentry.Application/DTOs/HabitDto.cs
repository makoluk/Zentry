using Zentry.Domain.Entities;

namespace Zentry.Application.DTOs;

/// <summary>
/// Habit data transfer object
/// </summary>
public class HabitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public HabitType Type { get; set; }
    public string? Unit { get; set; }
    public int? TargetValue { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    
    // Weekly completion data (7 days)
    public List<HabitEntryDto> WeeklyEntries { get; set; } = new();
}
