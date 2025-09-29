using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zentry.Application.Interfaces;
using Zentry.Domain.Common;
using Zentry.Domain.Entities;

namespace Zentry.Infrastructure.Data;

/// <summary>
/// Main database context for the application
/// </summary>
public class ZentryDbContext : DbContext, IAppDbContext
{
    public ZentryDbContext(DbContextOptions<ZentryDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitEntry> HabitEntries => Set<HabitEntry>();

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        // Apply configurations from Infrastructure assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZentryDbContext).Assembly);
        
        // Seed default categories
        SeedCategories(modelBuilder);
        SeedHabits(modelBuilder);

        // Configure global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var notExpression = Expression.Not(property);
                var lambda = Expression.Lambda(notExpression, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override int SaveChanges()
    {
        UpdateAuditableEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    break;
            }
        }
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new Category
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "İzlenecek Filmler",
                Description = "İzlemek istediğiniz filmler",
                Color = "#FF6B6B",
                Icon = "film",
                SortOrder = 1,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Günlük",
                Description = "Günlük yapılacak işler",
                Color = "#4ECDC4",
                Icon = "calendar-day",
                SortOrder = 2,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Bir Ara",
                Description = "Bir ara yapılacak işler",
                Color = "#45B7D1",
                Icon = "clock",
                SortOrder = 3,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Alışveriş",
                Description = "Alınacak ürünler",
                Color = "#96CEB4",
                Icon = "shopping-cart",
                SortOrder = 4,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Okuma",
                Description = "Okunacak kitaplar ve makaleler",
                Color = "#FFEAA7",
                Icon = "book",
                SortOrder = 5,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }

    private static void SeedHabits(ModelBuilder modelBuilder)
    {
        var habits = new[]
        {
            new Habit
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Name = "Su İçmek",
                Description = "Günde 8 bardak su içme hedefi",
                Color = "#3B82F6",
                Icon = "droplets",
                Type = HabitType.Numeric,
                Unit = "bardak",
                TargetValue = 8,
                SortOrder = 1,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Habit
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Name = "Egzersiz",
                Description = "Günlük spor yapma",
                Color = "#EF4444",
                Icon = "activity",
                Type = HabitType.Boolean,
                Unit = null,
                TargetValue = null,
                SortOrder = 2,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Habit
            {
                Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                Name = "Kitap Okuma",
                Description = "Günlük sayfa okuma hedefi",
                Color = "#10B981",
                Icon = "book",
                Type = HabitType.Numeric,
                Unit = "sayfa",
                TargetValue = 20,
                SortOrder = 3,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Habit
            {
                Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                Name = "Meditasyon",
                Description = "Günlük meditasyon pratiği",
                Color = "#8B5CF6",
                Icon = "brain",
                Type = HabitType.Boolean,
                Unit = null,
                TargetValue = null,
                SortOrder = 4,
                IsActive = true,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Habit>().HasData(habits);
    }
}

/// <summary>
/// Interface for timestamp provider (useful for testing)
/// </summary>
public interface ITimestampProvider
{
    DateTime UtcNow { get; }
}

/// <summary>
/// Default timestamp provider implementation
/// </summary>
public class TimestampProvider : ITimestampProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
