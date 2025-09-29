using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Habits.Commands.UpdateHabitEntry;

public class UpdateHabitEntryCommandHandler : IRequestHandler<UpdateHabitEntryCommand, Result<HabitEntryDto>>
{
    private readonly IAppDbContext _context;

    public UpdateHabitEntryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HabitEntryDto>> Handle(UpdateHabitEntryCommand request, CancellationToken cancellationToken)
    {
        // Check if habit exists
        var habit = await _context.Habits.FindAsync([request.HabitId], cancellationToken).ConfigureAwait(false);
        if (habit is null)
        {
            return Result.NotFound<HabitEntryDto>("Habit not found", "HABIT_NOT_FOUND");
        }

        // Find existing entry for this date
        var existingEntry = await _context.HabitEntries
            .FirstOrDefaultAsync(e => e.HabitId == request.HabitId && e.Date == request.Date, cancellationToken)
            .ConfigureAwait(false);

        HabitEntry entry;

        if (existingEntry is not null)
        {
            // Update existing entry
            existingEntry.IsCompleted = request.IsCompleted;
            existingEntry.Value = request.Value;
            existingEntry.Notes = request.Notes;
            existingEntry.UpdatedAtUtc = DateTime.UtcNow;
            
            _context.HabitEntries.Update(existingEntry);
            entry = existingEntry;
        }
        else
        {
            // Create new entry
            entry = new HabitEntry
            {
                HabitId = request.HabitId,
                Date = request.Date,
                IsCompleted = request.IsCompleted,
                Value = request.Value,
                Notes = request.Notes
            };
            
            _context.HabitEntries.Add(entry);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(entry.ToDto(), "Habit entry updated successfully");
    }
}
