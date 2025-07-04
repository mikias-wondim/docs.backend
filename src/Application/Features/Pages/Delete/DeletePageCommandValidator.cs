using FluentValidation;

namespace Application.Features.Pages.Delete;

internal sealed class DeletePageCommandValidator: AbstractValidator<DeletePageCommand>
{
    public DeletePageCommandValidator()
    {
        RuleFor(p => p.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");
    }
}
