using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Habits.Commands.UpdateHabitEntry;

public record UpdateHabitEntryCommand : IRequest<Result<HabitEntryDto>>
{
    public Guid HabitId { get; init; }
    public DateOnly Date { get; init; }
    public bool IsCompleted { get; init; }
    public int? Value { get; init; }
    public string? Notes { get; init; }
}
