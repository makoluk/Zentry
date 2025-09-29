using FluentValidation;

namespace Zentry.Application.Features.Tasks.Commands.ToggleTask;

/// <summary>
/// Validator for ToggleTaskCommand
/// </summary>
public class ToggleTaskCommandValidator : AbstractValidator<ToggleTaskCommand>
{
    public ToggleTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");
    }
}
