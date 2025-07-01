using Domain.Sections;
using FluentValidation;

namespace Application.Features.Sections.Update;

public class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {

        RuleFor(s => s.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section name is required.")
            .MaximumLength(SectionConstraints.MaxNameLength)
            .WithMessage($"Section name must not exceed {SectionConstraints.MaxNameLength} characters.");

        RuleFor(s => s.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section description is required.")
            .MaximumLength(SectionConstraints.MaxDescriptionLength)
            .WithMessage($"Section description must not exceed {SectionConstraints.MaxDescriptionLength} characters.");


        RuleFor(s => s.Visibility)
            .IsInEnum()
            .WithMessage("Section visibility is invalid.");

        When(s => s.Visibility == SectionVisibility.ProtectedWithPassword,
            () => RuleFor(s => s.Password)
                .NotEmpty().WithMessage("Password is required for password-protected sections.")
                .MaximumLength(SectionConstraints.MaxPasswordLength)
                .WithMessage($"Password must not exceed {SectionConstraints.MaxPasswordLength} characters."));

        When(s => s.Visibility == SectionVisibility.ProtectedByRole,
            () => RuleFor(s => s.AllowedRoles)
                .NotNull().WithMessage("Allowed roles must be provided for role-based protected sections.")
                .Must(r => r is { Count: > 0 })
                .WithMessage("At least one allowed role is required."));

        When(s => s.Visibility == SectionVisibility.ProtectedByUser,
            () => RuleFor(s => s.AllowedUserIds)
                .NotNull().WithMessage("Allowed users must be provided for user-restricted protected sections.")
                .Must(u => u is { Count: > 0 })
                .WithMessage("At least one allowed user is required."));
    }
}
