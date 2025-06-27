using Application.Abstractions.Messaging;

namespace Application.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword)
    : ICommand<bool>;
