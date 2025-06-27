using FluentValidation;

namespace Application.Features.ProjectMembers.Remove;

internal sealed class RemoveProjectMemberCommandValidator: AbstractValidator<RemoveProjectMemberCommand>
{
    public RemoveProjectMemberCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
        
        RuleFor(p => p.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("User ID is required.");
    }
}
