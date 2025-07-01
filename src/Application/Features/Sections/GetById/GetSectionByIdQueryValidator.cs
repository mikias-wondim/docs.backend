using FluentValidation;

namespace Application.Features.Sections.GetById;

internal sealed class GetSectionByIdQueryValidator: AbstractValidator<GetSectionByIdQuery>
{
    public GetSectionByIdQueryValidator()
    {
        RuleFor(s => s.SectionId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section ID is required.");
    }
}
