using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;

namespace Zentry.Application.Features.Habits.Queries.GetHabits;

public class GetHabitsQueryHandler : IRequestHandler<GetHabitsQuery, Result<List<HabitDto>>>
{
    private readonly IAppDbContext _context;

    public GetHabitsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<HabitDto>>> Handle(GetHabitsQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Habits.AsQueryable();

        // Apply filters
        if (request.IsActive.HasValue)
        {
            queryable = queryable.Where(h => h.IsActive == request.IsActive.Value);
        }

        // Calculate week range
        var weekStart = request.WeekStartDate ?? GetCurrentWeekStart();
        var weekEnd = weekStart.AddDays(6);

        // Get habits with their weekly entries
        var habits = await queryable
            .Include(h => h.Entries.Where(e => e.Date >= weekStart && e.Date <= weekEnd))
            .OrderBy(h => h.SortOrder)
            .ThenBy(h => h.Name)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        var habitDtos = habits.Select(h => h.ToDto()).ToList();

        return Result.Ok(habitDtos);
    }

    private static DateOnly GetCurrentWeekStart()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dayOfWeek = (int)today.DayOfWeek;
        var startOfWeek = today.AddDays(-dayOfWeek); // Pazar = 0, Pazartesi = haftanın başı için -dayOfWeek + 1
        return startOfWeek;
    }
}
