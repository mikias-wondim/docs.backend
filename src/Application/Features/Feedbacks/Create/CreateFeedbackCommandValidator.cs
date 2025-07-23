using Domain.Feedbacks;
using FluentValidation;

namespace Application.Features.Feedbacks.Create;

internal sealed class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("PageId is required.");
        
        RuleFor(x => x.Rating)
            .Must((rating) => rating is >= 1 and <= 5)
            .WithMessage("Rating must be between 1 and 5.");
        
        RuleFor(x => x.Comment).MaximumLength(FeedbackConstraints.MaxCommentLength)
            .WithMessage($"Comment must not exceed {FeedbackConstraints.MaxCommentLength} characters.");
    }
}
