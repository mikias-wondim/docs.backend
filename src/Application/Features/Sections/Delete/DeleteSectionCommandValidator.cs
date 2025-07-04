using FluentValidation;

namespace Application.Features.Sections.Delete;

internal sealed class DeleteSectionCommandValidator: AbstractValidator<DeleteSectionCommand>
{
    public DeleteSectionCommandValidator()
    {
        RuleFor(s => s.SectionId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section Id is required.");
    }
}
