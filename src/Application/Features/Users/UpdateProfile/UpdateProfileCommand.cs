using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? DisplayName,
    IFormFile? Avatar,
    string? Bio
) : ICommand<Guid>;
