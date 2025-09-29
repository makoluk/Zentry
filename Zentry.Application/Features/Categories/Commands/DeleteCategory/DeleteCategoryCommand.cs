using MediatR;
using Zentry.Application.Common;

namespace Zentry.Application.Features.Categories.Commands.DeleteCategory;

/// <summary>
/// Command to delete a category
/// </summary>
public record DeleteCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
