using FluentValidation;

namespace Application.Features.Media.Delete;

internal sealed class DeleteSectionAssetCommandValidator : AbstractValidator<DeleteSectionAssetCommand>
{
    public DeleteSectionAssetCommandValidator()
    {
        RuleFor(c => c.SectionId)
            .NotEmpty()
            .WithMessage("SectionId is required.");

        RuleFor(c => c.RelativePath)
            .NotEmpty()
            .WithMessage("RelativePath is required.")
            .MaximumLength(255)
            .WithMessage("RelativePath must be less than 255 characters.");
    }
}
