namespace Application.Abstractions.Authentication;

public sealed class AccessToken(string token, DateTime expiresAt)
{
    public string Token { get; init; } = token;
    public DateTime ExpiresAt { get; init; } = expiresAt;
}
