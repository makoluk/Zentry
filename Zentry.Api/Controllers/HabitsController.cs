using Microsoft.AspNetCore.Mvc;
using Zentry.Api.Extensions;
using Zentry.Application.DTOs;
using Zentry.Application.Features.Habits.Commands.CreateHabit;
using Zentry.Application.Features.Habits.Commands.DeleteHabit;
using Zentry.Application.Features.Habits.Commands.ReorderHabits;
using Zentry.Application.Features.Habits.Commands.UpdateHabit;
using Zentry.Application.Features.Habits.Commands.UpdateHabitEntry;
using Zentry.Application.Features.Habits.Queries.GetHabits;
using MediatR;

namespace Zentry.Api.Controllers;

/// <summary>
/// Habits API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class HabitsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get habits with weekly data
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<HabitDto>>> GetHabits(
        [FromQuery] bool? isActive = true,
        [FromQuery] DateOnly? weekStartDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetHabitsQuery
        {
            IsActive = isActive,
            WeekStartDate = weekStartDate
        };

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Create a new habit
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        [FromBody] CreateHabitCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Update a habit
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<HabitDto>> UpdateHabit(
        Guid id,
        [FromBody] UpdateHabitCommand command,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"UpdateHabit called with ID: {id}");
        ArgumentNullException.ThrowIfNull(command);
        var updateCommand = command with { Id = id };
        Console.WriteLine($"Sending command with ID: {updateCommand.Id}");
        var result = await _mediator.Send(updateCommand, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"Result success: {result.Success}");

        return this.FromResult(result);
    }

    /// <summary>
    /// Reorder habits
    /// </summary>
    [HttpPut("reorder")]
    public async Task<ActionResult> ReorderHabits(
        [FromBody] ReorderHabitsCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Update or create habit entry for a specific date
    /// </summary>
    [HttpPut("{habitId:guid}/entries")]
    public async Task<ActionResult<HabitEntryDto>> UpdateHabitEntry(
        Guid habitId,
        [FromBody] UpdateHabitEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var updateCommand = command with { HabitId = habitId };
        var result = await _mediator.Send(updateCommand, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }

    /// <summary>
    /// Delete a habit
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteHabit(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteHabitCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return this.FromResult(result);
    }
}