using FluentValidation;

namespace Application.Features.Faqs.Move;

internal sealed class MoveFaqCommandValidator: AbstractValidator<MoveFaqCommand>
{
    public MoveFaqCommandValidator()
    {
        RuleFor(f => f.FaqId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Faq ID is required.");
        
        RuleFor(f => f.NewIndex)
            .NotNull()
            .WithMessage("New Index is required.");
    }
}
