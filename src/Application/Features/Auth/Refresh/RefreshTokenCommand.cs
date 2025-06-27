using Application.Abstractions.Messaging;
using Application.Features.Users;

namespace Application.Features.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken): ICommand<AuthLoginResponse>;
