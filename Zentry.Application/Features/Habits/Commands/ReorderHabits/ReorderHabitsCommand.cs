using MediatR;
using Zentry.Application.Common;

namespace Zentry.Application.Features.Habits.Commands.ReorderHabits;

/// <summary>
/// Command to reorder habits
/// </summary>
public record ReorderHabitsCommand : IRequest<Result>
{
    public List<HabitOrderItem> Habits { get; init; } = [];
}

/// <summary>
/// Habit order item
/// </summary>
public record HabitOrderItem
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
}
