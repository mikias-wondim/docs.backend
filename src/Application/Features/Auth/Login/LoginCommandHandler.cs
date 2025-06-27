using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Users;
using AutoMapper;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IMapper mapper) : ICommandHandler<LoginCommand, AuthLoginResponse>
{
    public async Task<Result<AuthLoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthLoginResponse>(AuthErrors.InvalidCredential);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<AuthLoginResponse>(AuthErrors.InvalidCredential);
        }

        AccessToken accessToken = tokenProvider.Create(user);

        DateTime refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            user.Id,
            tokenProvider.GenerateRandomToken(),
            refreshTokenExpiresAt);
        
        await context.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);
        
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        var response = new AuthLoginResponse(
            mapper.Map<UserResponse>(user),
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken.Token,
            refreshTokenExpiresAt
            );

        return response;
    }
}
