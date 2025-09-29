using MediatR;
using Zentry.Application.Common;
using Zentry.Application.DTOs;

namespace Zentry.Application.Features.Habits.Queries.GetHabits;

public record GetHabitsQuery : IRequest<Result<List<HabitDto>>>
{
    public bool? IsActive { get; init; } = true;
    public DateOnly? WeekStartDate { get; init; } // Hangi haftanın verilerini getireceğiz
}
