using Application.Features.Users;

namespace Application.Features.Auth;

public sealed record AuthLoginResponse(
    UserResponse User,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
