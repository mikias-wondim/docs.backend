using Domain.Users;

namespace Domain.Auth;

public class EmailVerificationToken(Guid id, Guid userId, string token, DateTime createdAtUtc, DateTime expiresAtUtc)
{
    public Guid Id { get; private set; } = id;
    public Guid UserId { get; private set; } = userId;
    public string Token { get; private set; } = token;
    public DateTime CreatedAtUtc { get; private set; } = createdAtUtc;
    public DateTime ExpiresAtUtc { get; private set; } = expiresAtUtc;

    public User User { get; init; }
}
