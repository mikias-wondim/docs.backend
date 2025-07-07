using FluentValidation;

namespace Application.Features.Pages.GetBySectionId;

internal sealed class GetPagesBySectionIdQueryValidator : AbstractValidator<GetPagesBySectionIdQuery>
{
    public GetPagesBySectionIdQueryValidator()
    {
        RuleFor( p => p.SectionId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section ID is required.");
    }
}
