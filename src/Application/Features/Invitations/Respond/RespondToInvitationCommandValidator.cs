using FluentValidation;

namespace Application.Features.Invitations.Respond;

internal sealed class RespondToInvitationCommandValidator: AbstractValidator<RespondToInvitationCommand>
{
    public RespondToInvitationCommandValidator()
    {
        RuleFor(p => p.Token)
            .NotEmpty()
            .NotNull()
            .WithMessage("Token is required.");
        
        RuleFor(p => p.Accept)
            .NotEmpty()
            .NotNull()
            .WithMessage("Accept is required.");
    }   
}
