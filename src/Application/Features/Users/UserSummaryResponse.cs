namespace Application.Features.Users;

public sealed class UserSummaryResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}
