using FluentValidation;

namespace Application.Features.Sections.ChangeDefaultPage;

internal sealed class ChangeDefaultPageCommandValidator: AbstractValidator<ChangeDefaultPageCommand>
{
    public ChangeDefaultPageCommandValidator()
    {
        RuleFor(s => s.SectionId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section Id is required.");
        
        RuleFor(s => s.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page Id is required.");
    }
}
