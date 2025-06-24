using Domain.Projects;
using FluentValidation;

namespace Application.Features.Projects.UpdateOverview;

internal sealed class UpdateProjectOverviewCommandValidator : AbstractValidator<UpdateProjectOverviewCommand>
{
    public UpdateProjectOverviewCommandValidator()
    {
        RuleFor(c => c.ProjectId)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(c => c.Overview)
            .NotEmpty().WithMessage("Overview is required.")
            .MaximumLength(ProjectConstraints.MaxOverviewMdLength)
            .WithMessage($"Project name must not exceed {ProjectConstraints.MaxOverviewMdLength} characters.");
    }
}
