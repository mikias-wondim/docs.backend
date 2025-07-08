using FluentValidation;

namespace Application.Features.Media.Get;

internal sealed class GetSectionAssetQueryValidator : AbstractValidator<GetSectionAssetQuery>
{
    public GetSectionAssetQueryValidator()
    {
        RuleFor(q => q.SectionId)
            .NotEmpty()
            .WithMessage("SectionId is required.");

        RuleFor(q => q.RelativePath)
            .NotEmpty()
            .WithMessage("RelativePath is required.")
            .MaximumLength(255)
            .WithMessage("RelativePath must be less than 255 characters.");
    }
}
