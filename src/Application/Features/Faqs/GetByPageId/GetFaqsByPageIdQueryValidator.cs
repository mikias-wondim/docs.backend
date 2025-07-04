using FluentValidation;

namespace Application.Features.Faqs.GetByPageId;

internal sealed class GetFaqsByPageIdQueryValidator : AbstractValidator<GetFaqsByPageIdQuery>
{
    public GetFaqsByPageIdQueryValidator()
    {
        RuleFor(x => x.PageId)
            .NotNull()
            .NotEmpty()
            .WithMessage("PageId is required.");
    }
}
