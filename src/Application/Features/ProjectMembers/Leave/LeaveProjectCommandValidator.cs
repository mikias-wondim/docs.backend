using FluentValidation;

namespace Application.Features.ProjectMembers.Leave;

internal sealed class LeaveProjectCommandValidator: AbstractValidator<LeaveProjectCommand>
{
    public LeaveProjectCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
    }
}
