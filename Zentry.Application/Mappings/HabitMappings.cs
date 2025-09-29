using Zentry.Application.DTOs;
using Zentry.Domain.Entities;

namespace Zentry.Application.Mappings;

public static class HabitMappings
{
    public static HabitDto ToDto(this Habit habit)
    {
        return new HabitDto
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Color = habit.Color,
            Icon = habit.Icon,
            Type = habit.Type,
            Unit = habit.Unit,
            TargetValue = habit.TargetValue,
            IsActive = habit.IsActive,
            SortOrder = habit.SortOrder,
            CreatedAtUtc = habit.CreatedAtUtc,
            UpdatedAtUtc = habit.UpdatedAtUtc,
            WeeklyEntries = habit.Entries?.Select(e => e.ToDto()).ToList() ?? new List<HabitEntryDto>()
        };
    }
    
    public static HabitEntryDto ToDto(this HabitEntry entry)
    {
        return new HabitEntryDto
        {
            Id = entry.Id,
            HabitId = entry.HabitId,
            Date = entry.Date,
            IsCompleted = entry.IsCompleted,
            Value = entry.Value,
            Notes = entry.Notes,
            CreatedAtUtc = entry.CreatedAtUtc,
            UpdatedAtUtc = entry.UpdatedAtUtc
        };
    }
}
