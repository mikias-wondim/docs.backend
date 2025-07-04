using Domain.Pages;
using FluentValidation;

namespace Application.Features.Pages.Create;

internal sealed class CreatePageCommandValidator: AbstractValidator<CreatePageCommand>
{
    public CreatePageCommandValidator()
    {
        RuleFor(p => p.SectionId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Section ID is required.");
        
        RuleFor(p => p.Title)
            .NotEmpty()
            .NotNull()
            .WithMessage("Title is required.");

        RuleFor(p => p.ContentMd)
            .MaximumLength(PagesConstraints.MaxContentMdLength)
            .WithMessage($"Content MD must not exceed {PagesConstraints.MaxContentMdLength} characters.");
    }
}
