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
    IMapper mapper): ICommandHandler<RefreshTokenCommand, UserLoginResponse>
{
    public async Task<Result<UserLoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken existingToken = await context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == command.RefreshToken, cancellationToken: cancellationToken);

        if (existingToken is null)
        {
            return Result.Failure<UserLoginResponse>(AuthErrors.InvalidRefreshToken);
        }

        if (existingToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Failure<UserLoginResponse>(AuthErrors.ExpiredRefreshToken);
        }
        
        User user = existingToken.User;
        
        string accessToken = tokenProvider.Create(existingToken.User);
        var newRefreshToken = new RefreshToken(
            Guid.NewGuid(),
            existingToken.UserId,
            tokenProvider.GenerateRandomToken(),
            DateTime.UtcNow.AddDays(7));
        
        context.RefreshTokens.Add(newRefreshToken);
        context.RefreshTokens.Remove(existingToken);
        await context.SaveChangesAsync(cancellationToken);
        
        var response = new UserLoginResponse(
            mapper.Map<UserResponse>(user),
            accessToken,
            newRefreshToken.Token);
        
        return response;
    }
}
