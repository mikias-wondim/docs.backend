using FluentValidation;

namespace Application.Features.ProjectMembers.UpdateRole;

internal sealed class UpdateProjectMemberRoleCommandValidator: AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
        
        RuleFor(p => p.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("User ID is required.");
        
        RuleFor(p => p.NewRole)
            .NotEmpty()
            .NotNull()
            .WithMessage("New role is required.")
            .IsInEnum()
            .WithMessage("Invalid role.");       
    }
}
