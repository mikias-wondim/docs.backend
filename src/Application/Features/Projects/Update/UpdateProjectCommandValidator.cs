using FluentValidation;

namespace Application.Features.Projects.Update;

public class UpdateProjectCommandValidator: AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.ProjectId)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.");

        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(c => !string.IsNullOrWhiteSpace(c.Description));

        RuleFor(c => c.Visibility)
            .IsInEnum().WithMessage("Invalid project visibility.");
    }
}
