using FluentValidation;

namespace Application.Features.Projects.Get;

internal sealed class GetProjectsQueryValidator: AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000).WithMessage("PageSize must be between 1 and 100.");
    }
}
