using Zentry.Application.Features.Tasks.Commands.CreateTask;
using Zentry.Application.Features.Tasks.Commands.UpdateTask;
using Zentry.Application.DTOs;
using Zentry.Domain.Entities;

namespace Zentry.Application.Mappings;

/// <summary>
/// Manual mapping extensions for Task entities and DTOs
/// </summary>
public static class TaskMappings
{
    public static TaskDto ToDto(this TaskItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsDone = entity.IsDone,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category.Name,
            CategoryColor = entity.Category.Color
        };
    }

    public static TaskItem ToEntity(this CreateTaskCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return new TaskItem
        {
            Title = command.Title,
            Description = command.Description,
            IsDone = false,
            CategoryId = command.CategoryId
        };
    }

    public static void UpdateFromDto(this TaskItem entity, UpdateTaskCommand command)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(command);
        entity.Title = command.Title;
        entity.Description = command.Description;
        entity.IsDone = command.IsDone;
        entity.CategoryId = command.CategoryId;
    }
}
