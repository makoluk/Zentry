using MediatR;
using Zentry.Application.Common;

namespace Zentry.Application.Features.Habits.Commands.DeleteHabit;

/// <summary>
/// Command to delete a habit
/// </summary>
public record DeleteHabitCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
