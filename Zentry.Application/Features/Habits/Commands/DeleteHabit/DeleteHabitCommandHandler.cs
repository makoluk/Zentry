using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.Interfaces;

namespace Zentry.Application.Features.Habits.Commands.DeleteHabit;

/// <summary>
/// Handler for DeleteHabitCommand
/// </summary>
public class DeleteHabitCommandHandler : IRequestHandler<DeleteHabitCommand, Result>
{
    private readonly IAppDbContext _context;

    public DeleteHabitCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
    {
        var habit = await _context.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken).ConfigureAwait(false);
        
        if (habit is null)
        {
            return Result.NotFound("Habit not found", "HABIT_NOT_FOUND");
        }

        // Remove all habit entries first
        if (habit.Entries.Any())
        {
            _context.HabitEntries.RemoveRange(habit.Entries);
        }

        // Remove the habit
        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.NoContent();
    }
}
