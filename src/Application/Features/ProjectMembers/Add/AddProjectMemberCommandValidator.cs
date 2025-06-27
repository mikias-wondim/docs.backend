using FluentValidation;

namespace Application.Features.ProjectMembers.Add;

internal sealed class AddProjectMemberCommandValidator: AbstractValidator<AddProjectMemberCommand>
{
    public AddProjectMemberCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
    }
}
