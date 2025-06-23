using Application.Abstractions.Messaging;
using Application.Features.Users;

namespace Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<UserLoginResponse>;
