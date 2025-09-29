using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Habits.Commands.UpdateHabit;

public record UpdateHabitCommand : IRequest<Result<HabitDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Color { get; init; } = "#3B82F6";
    public string Icon { get; init; } = "target";
    public HabitType Type { get; init; } = HabitType.Boolean;
    public string? Unit { get; init; }
    public int? TargetValue { get; init; }
    public bool IsActive { get; init; } = true;
}
