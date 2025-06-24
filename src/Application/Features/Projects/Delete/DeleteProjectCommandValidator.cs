using FluentValidation;

namespace Application.Features.Projects.Delete;

internal sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(c => c.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
    }
}
