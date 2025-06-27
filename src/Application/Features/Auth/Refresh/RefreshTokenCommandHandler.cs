using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Users;
using AutoMapper;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auth.Refresh;

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext context,
    ITokenProvider tokenProvider,
    IMapper mapper) : ICommandHandler<RefreshTokenCommand, AuthLoginResponse>
{
    public async Task<Result<AuthLoginResponse>> Handle(RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        RefreshToken existingToken = await context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == command.RefreshToken, cancellationToken: cancellationToken);

        if (existingToken is null)
        {
            return Result.Failure<AuthLoginResponse>(AuthErrors.InvalidRefreshToken);
        }

        if (existingToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Failure<AuthLoginResponse>(AuthErrors.ExpiredRefreshToken);
        }

        User user = existingToken.User;

        AccessToken accessToken = tokenProvider.Create(existingToken.User);
        
        DateTime refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        var newRefreshToken = new RefreshToken(
            Guid.NewGuid(),
            user.Id,
            tokenProvider.GenerateRandomToken(),
            refreshTokenExpiresAt);

        await context.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);
        
        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync(cancellationToken);

        var response = new AuthLoginResponse(
            mapper.Map<UserResponse>(user),
            accessToken.Token,
            accessToken.ExpiresAt,
            newRefreshToken.Token,
            refreshTokenExpiresAt
        );

        return response;
    }
}
