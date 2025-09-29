namespace Zentry.Application.DTOs;

/// <summary>
/// Habit entry data transfer object
/// </summary>
public class HabitEntryDto
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
    public bool IsCompleted { get; set; }
    public int? Value { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
