using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Interfaces;
using Zentry.Application.Mappings;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Habits.Commands.CreateHabit;

public class CreateHabitCommandHandler : IRequestHandler<CreateHabitCommand, Result<HabitDto>>
{
    private readonly IAppDbContext _context;

    public CreateHabitCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HabitDto>> Handle(CreateHabitCommand request, CancellationToken cancellationToken)
    {
        // Get the next available SortOrder
        var maxSortOrder = 0;
        var existingHabits = await _context.Habits
            .Where(h => h.IsActive)
            .Select(h => h.SortOrder)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        
        if (existingHabits.Count > 0)
        {
            maxSortOrder = existingHabits.Max();
        }

        var habit = new Habit
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            Type = request.Type,
            Unit = request.Unit,
            TargetValue = request.TargetValue,
            SortOrder = request.SortOrder > 0 ? request.SortOrder : maxSortOrder + 1,
            IsActive = request.IsActive
        };

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Created(habit.ToDto(), "Habit created successfully");
    }
}
