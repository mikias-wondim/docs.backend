using FluentValidation;

namespace Application.Features.Users.Get;

internal sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThan(0);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);

        RuleFor(q => q.SortBy)
            .Must(BeAValidSortByField)
            .WithMessage("Invalid sort field");

        RuleFor(q => q.SortOrder)
            .Must(v => v is null || v.ToLower(System.Globalization.CultureInfo.CurrentCulture) is "asc" or "desc")
            .WithMessage("Sort order must be 'asc' or 'desc'");
    }

    private static readonly string[] ValidSortOptions = ["createdAt", "email", "firstName", "lastName"];

    private static bool BeAValidSortByField(string? field)
    {
        return string.IsNullOrWhiteSpace(field) ||
               ValidSortOptions.Contains(field.ToLower(System.Globalization.CultureInfo.CurrentCulture));
    }
}
