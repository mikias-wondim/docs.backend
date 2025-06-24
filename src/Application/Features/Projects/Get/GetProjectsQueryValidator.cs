using FluentValidation;

namespace Application.Features.Projects.Get;

internal sealed class GetProjectsQueryValidator: AbstractValidator<GetProjectsQuery>
{
    private static readonly string[] AllowedSortByFields = ["name", "createdat", "updatedat"];
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .WithMessage("Project name filter must not exceed 50 characters.");

        RuleFor(x => x.SortBy)
            .Must(BeAValidSortByField)
            .WithMessage($"SortBy must be one of the following: {string.Join(", ", AllowedSortByFields)}");

        RuleFor(x => x.SortOrder)
            .Must(order =>
                order is null ||
                AllowedSortOrders.Contains(order.ToLower(System.Globalization.CultureInfo.CurrentCulture)))
            .WithMessage($"SortOrder must be one of: {string.Join(", ", AllowedSortOrders)}");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
    }

    private static bool BeAValidSortByField(string? field)
    {
        return string.IsNullOrWhiteSpace(field) ||
               AllowedSortByFields.Contains(field.ToLower(System.Globalization.CultureInfo.CurrentCulture));
    }
}
