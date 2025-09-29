using Microsoft.EntityFrameworkCore;
using Zentry.Domain.Entities;

namespace Zentry.Application.Interfaces;

/// <summary>
/// Application database context interface for dependency injection and testing
/// </summary>
public interface IAppDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DbSet<Category> Categories { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Habit> Habits { get; }
    DbSet<HabitEntry> HabitEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
