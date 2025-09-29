using Microsoft.AspNetCore.Mvc;
using Zentry.Api.Extensions;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Features.Tasks.Commands.CreateTask;
using Zentry.Application.Features.Tasks.Commands.UpdateTask;
using Zentry.Application.Features.Tasks.Commands.DeleteTask;
using Zentry.Application.Features.Tasks.Commands.ToggleTask;
using Zentry.Application.Features.Tasks.Queries.GetTasks;
using Zentry.Application.Features.Tasks.Queries.GetTaskById;
using MediatR;

namespace Zentry.Api.Controllers;

/// <summary>
/// Tasks API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class TasksController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get paginated list of tasks
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isDone = null,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksQuery
        {
            Page = page,
            PageSize = pageSize,
            IsDone = isDone,
            Search = search,
            CategoryId = categoryId
        };

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTaskByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Data.Id }, result.Data);
        }

        return this.FromResult(result);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var updateCommand = command with { Id = id };
        var result = await _mediator.Send(updateCommand, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Toggle task completion status
    /// </summary>
    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult<TaskDto>> ToggleTask(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new ToggleTaskCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTask(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTaskCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }
}
