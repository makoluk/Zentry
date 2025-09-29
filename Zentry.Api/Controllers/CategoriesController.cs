using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zentry.Api.Extensions;
using Zentry.Application.DTOs;
using Zentry.Application.Features.Categories.Commands.CreateCategory;
using Zentry.Application.Features.Categories.Commands.DeleteCategory;
using Zentry.Application.Features.Categories.Commands.ReorderCategories;
using Zentry.Application.Features.Categories.Commands.UpdateCategory;
using Zentry.Application.Features.Categories.Queries.GetCategories;
using Zentry.Application.Features.Categories.Queries.GetCategoryById;

namespace Zentry.Api.Controllers;

/// <summary>
/// Controller for managing categories
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories([FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery { IsActive = isActive };
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }

    /// <summary>
    /// Get a category by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await _mediator.Send(updateCommand, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }

    /// <summary>
    /// Reorder categories
    /// </summary>
    [HttpPut("reorder")]
    public async Task<ActionResult> ReorderCategories([FromBody] ReorderCategoriesCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return this.FromResult(result);
    }
}
