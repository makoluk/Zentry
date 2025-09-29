using FluentValidation;

namespace Zentry.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Validator for DeleteTaskCommand
/// </summary>
public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");
    }
}
