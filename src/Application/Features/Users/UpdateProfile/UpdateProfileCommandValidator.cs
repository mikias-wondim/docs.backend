using Domain.Users;
using FluentValidation;

namespace Application.Features.Users.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(UserConstraints.MaxFirstNameLength)
            .WithMessage($"First name must not exceed {UserConstraints.MaxFirstNameLength} characters.");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(UserConstraints.MaxLastNameLength)
            .WithMessage($"Last name must not exceed {UserConstraints.MaxLastNameLength} characters.");

        RuleFor(c => c.DisplayName)
            .MaximumLength(UserConstraints.MaxDisplayNameLength)
            .WithMessage($"Display name must not exceed {UserConstraints.MaxDisplayNameLength} characters.");

        RuleFor(c => c.Bio)
            .MaximumLength(UserConstraints.MaxBioLength)
            .WithMessage($"Bio must not exceed {UserConstraints.MaxBioLength} characters.");

        When(c => c.Avatar is not null, () =>
        {
            RuleFor(c => c.Avatar!.Length)
                .LessThanOrEqualTo(UserConstraints.MaxFileSizeInBytes)
                .WithMessage($"Avatar must not exceed {UserConstraints.MaxFileSizeInBytes / (1024 * 1024):F1} MB.");

            RuleFor(c => c.Avatar!.ContentType)
                .Must(contentType => UserConstraints.AllowedFileTypes.Contains(contentType))
                .WithMessage($"Avatar must be one of the following file types: {
                    string.Join(", ", UserConstraints.AllowedFileTypes)
                }.");
        });
    }
}
