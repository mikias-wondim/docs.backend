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
    IMapper mapper) : ICommandHandler<LoginCommand, UserLoginResponse>
{
    public async Task<Result<UserLoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserLoginResponse>(AuthErrors.InvalidCredential);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<UserLoginResponse>(AuthErrors.InvalidCredential);
        }

        string accessToken = tokenProvider.Create(user);

        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            user.Id,
            tokenProvider.GenerateRandomToken(),
            DateTime.UtcNow.AddDays(7));
        
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        var response = new UserLoginResponse(
            mapper.Map<UserResponse>(user),
            accessToken,
            refreshToken.Token);
        
        return response;
    }
}
