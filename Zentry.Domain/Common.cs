using System.ComponentModel.DataAnnotations;

namespace Zentry.Domain.Common;

/// <summary>
/// Base entity with common properties for all domain entities
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

/// <summary>
/// Interface for entities that support soft delete
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}

/// <summary>
/// Interface for entities that need audit information
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
}

/// <summary>
/// Base entity with soft delete capability
/// </summary>
public abstract class BaseEntityWithSoftDelete : BaseEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Guard class for parameter validation
/// </summary>
public static class Guard
{
    public static void AgainstNull<T>(T value, string parameterName) where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void AgainstNullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null or empty.", parameterName);
        }
    }

    public static void AgainstNullOrWhiteSpace(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null, empty, or whitespace.", parameterName);
        }
    }

    public static void AgainstOutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Parameter '{parameterName}' must be between {min} and {max}.");
        }
    }

    public static void AgainstNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentException($"Parameter '{parameterName}' cannot be negative.", parameterName);
        }
    }

    public static void AgainstNegativeOrZero(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"Parameter '{parameterName}' must be greater than zero.", parameterName);
        }
    }
}
