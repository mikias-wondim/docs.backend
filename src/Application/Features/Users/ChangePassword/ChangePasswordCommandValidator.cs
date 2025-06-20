using FluentValidation;

namespace Application.Features.Users.ChangePassword;

public class ChangePasswordCommandValidator: AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(c => c.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.")
            .MinimumLength(8).WithMessage("Current password must be at least 8 characters.")
            .MaximumLength(32).WithMessage("Current password must not exceed 32 characters.");
        
        RuleFor(c => c.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
            .MaximumLength(32).WithMessage("New password must not exceed 32 characters.");
    }   
}
