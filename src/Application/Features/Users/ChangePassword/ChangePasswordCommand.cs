using Application.Abstractions.Messaging;

namespace Application.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword)
    : ICommand<bool>;
