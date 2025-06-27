using FluentValidation;

namespace Application.Features.Projects.TransferOwnership;

internal sealed class TransferProjectOwnershipCommandValidator: AbstractValidator<TransferProjectOwnershipCommand>
{
    public TransferProjectOwnershipCommandValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Project ID is required.");
        
        RuleFor(p => p.NewOwnerId)
            .NotEmpty()
            .NotNull()
            .WithMessage("New Owner User ID is required.");
    }
}
