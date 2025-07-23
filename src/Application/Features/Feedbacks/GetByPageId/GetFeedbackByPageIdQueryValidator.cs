using FluentValidation;

namespace Application.Features.Feedbacks.GetByPageId;

internal sealed class GetFeedbackByPageIdQueryValidator: AbstractValidator<GetFeedbackByPageIdQuery>
{
    public GetFeedbackByPageIdQueryValidator()
    {
        RuleFor(f => f.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");
    }
}
