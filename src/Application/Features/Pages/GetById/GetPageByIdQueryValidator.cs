using FluentValidation;

namespace Application.Features.Pages.GetById;

internal sealed class GetPageByIdQueryValidator: AbstractValidator<GetPageByIdQuery>
{
    public GetPageByIdQueryValidator()
    {
        RuleFor(p => p.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");
    }
}
