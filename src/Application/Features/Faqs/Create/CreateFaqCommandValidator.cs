using Domain.Faqs;
using FluentValidation;

namespace Application.Features.Faqs.Create;

internal sealed class CreateFaqCommandValidator: AbstractValidator<CreateFaqCommand>
{
    public CreateFaqCommandValidator()
    {
        RuleFor(f => f.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");
        
        RuleFor(f => f.Question)
            .NotEmpty()
            .NotNull()
            .WithMessage("Question is required.")
            .MaximumLength(FaqConstraints.MaxQuestionLength)
            .WithMessage($"Question must not exceed {FaqConstraints.MaxQuestionLength} characters.");
        
        RuleFor(f => f.Answer)
            .NotEmpty()
            .NotNull()
            .WithMessage("Answer is required.")
            .MaximumLength(FaqConstraints.MaxAnswerLength)
            .WithMessage($"Answer must not exceed {FaqConstraints.MaxAnswerLength} characters.");
    }
}
