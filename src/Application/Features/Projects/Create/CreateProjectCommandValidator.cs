using FluentValidation;

namespace Application.Features.Projects.Create;

public class CreateProjectCommandValidator: AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(c => c.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required.");

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
