using Domain.Pages;
using FluentValidation;

namespace Application.Features.Pages.Update;

internal sealed class UpdatePageCommandValidator: AbstractValidator<UpdatePageCommand>
{
    public UpdatePageCommandValidator()
    {
        RuleFor(p => p.PageId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Page ID is required.");
        
        RuleFor(p => p.Title)
            .NotEmpty()
            .NotNull()
            .WithMessage("Title is required.");

        RuleFor(p => p.ContentMd)
            .MaximumLength(PagesConstraints.MaxContentMdLength)
            .WithMessage($"Content MD must not exceed {PagesConstraints.MaxContentMdLength} characters.");
    }   
}
