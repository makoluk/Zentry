using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Habits.Commands.ReorderHabits;

/// <summary>
/// Handler for ReorderHabitsCommand
/// </summary>
public class ReorderHabitsCommandHandler : IRequestHandler<ReorderHabitsCommand, Result>
{
    private readonly IAppDbContext _context;

    public ReorderHabitsCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ReorderHabitsCommand request, CancellationToken cancellationToken)
    {
        if (request.Habits.Count == 0)
        {
            return Result.BadRequest("No habits provided for reordering", "EMPTY_HABITS_LIST");
        }

        // Get all habit IDs from the request
        var habitIds = request.Habits.Select(h => h.Id).ToList();
        
        // Fetch existing habits
        var existingHabits = await _context.Habits
            .Where(h => habitIds.Contains(h.Id))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (existingHabits.Count != request.Habits.Count)
        {
            return Result.BadRequest("Some habits not found", "HABITS_NOT_FOUND");
        }

        // Update sort order for each habit
        foreach (var habitOrder in request.Habits)
        {
            var habit = existingHabits.First(h => h.Id == habitOrder.Id);
            habit.SortOrder = habitOrder.SortOrder;
            habit.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok("Habits reordered successfully");
    }
}
