using Domain.Media;
using FluentValidation;

namespace Application.Features.Media.Upload;

internal sealed class UploadSectionAssetCommandValidator: AbstractValidator<UploadSectionAssetCommand>
{


    public UploadSectionAssetCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty().WithMessage("Section ID is required.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(file => file is { Length: > 0 })
            .WithMessage("Uploaded file must not be empty.")
            .Must(file => MediaAssetConstraints.AllowedContentTypes.Contains(file!.ContentType))
            .WithMessage($"Invalid file type. Allowed types: {string.Join(", ", MediaAssetConstraints.AllowedContentTypes)}")
            .Must(file => file!.Length <= MediaAssetConstraints.MaxFileSizeInBytes)
            .WithMessage($"File size must not exceed {MediaAssetConstraints.MaxFileSizeInBytes}MB.");
    }
}
