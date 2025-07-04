using FluentValidation;

namespace Application.Features.Pages.Move;

internal sealed class MovePageCommandValidator: AbstractValidator<MovePageCommand>
{
    public MovePageCommandValidator()
    {
        RuleFor(p => p.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");

        RuleFor(p => p.NewIndex)
            .NotNull()
            .WithMessage("New Index is required.");
    }
}
