namespace Application.Features.Users;

public sealed record UserLoginResponse(
    UserResponse User,
    string AccessToken,
    string RefreshToken);
