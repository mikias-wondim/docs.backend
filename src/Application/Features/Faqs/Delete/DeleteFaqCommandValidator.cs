using FluentValidation;

namespace Application.Features.Faqs.Delete;

internal sealed class DeleteFaqCommandValidator: AbstractValidator<DeleteFaqCommand>
{
    public DeleteFaqCommandValidator()
    {
        RuleFor(f => f.FaqId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Faq ID is required.");
    }
}
