using FluentValidation;

namespace Application.Features.Invitations.Send;

internal sealed class SendInvitationCommandValidator: AbstractValidator<SendInvitationCommand>
{
    public SendInvitationCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
        
        RuleFor(p => p.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("User ID is required.");
        
        RuleFor(p => p.Role)
            .IsInEnum()
            .WithMessage("Role is invalid.");
    }
}
