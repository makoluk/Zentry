using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;

namespace Zentry.Application.Features.Habits.Commands.UpdateHabit;

public class UpdateHabitCommandHandler : IRequestHandler<UpdateHabitCommand, Result<HabitDto>>
{
    private readonly IAppDbContext _context;

    public UpdateHabitCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HabitDto>> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Looking for habit with ID: {request.Id}");
        var habit = await _context.Habits.FindAsync([request.Id], cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"Found habit: {habit?.Name ?? "null"}");
        if (habit is null)
        {
            return Result.NotFound<HabitDto>("Habit not found", "HABIT_NOT_FOUND");
        }

        // Check if habit type is changing
        var typeChanged = habit.Type != request.Type;
        
        // If type is changing, clear existing habit entries to avoid data inconsistency
        if (typeChanged)
        {
            var existingEntries = await _context.HabitEntries
                .Where(e => e.HabitId == request.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            
            if (existingEntries.Any())
            {
                _context.HabitEntries.RemoveRange(existingEntries);
            }
        }

        // Update habit properties
        habit.Name = request.Name;
        habit.Description = request.Description;
        habit.Color = request.Color;
        habit.Icon = request.Icon;
        habit.Type = request.Type;
        habit.Unit = request.Unit;
        habit.TargetValue = request.TargetValue;
        habit.IsActive = request.IsActive;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        _context.Habits.Update(habit);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var message = typeChanged 
            ? "Habit updated successfully. Existing entries were cleared due to type change." 
            : "Habit updated successfully";

        return Result.Ok(habit.ToDto(), message);
    }
}
