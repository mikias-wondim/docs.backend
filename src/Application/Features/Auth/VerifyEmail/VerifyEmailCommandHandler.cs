using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<VerifyEmailCommand, bool>
{
    public async Task<Result<bool>> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        EmailVerificationToken verificationToken = await context.EmailVerificationTokens
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Token == command.Token, cancellationToken);
        
        if (verificationToken is null)
        {
            return Result.Failure<bool>(AuthErrors.InvalidEmailVerificationToken);
        }

        if (verificationToken.ExpiresAtUtc < dateTimeProvider.UtcNow)
        {
            return Result.Failure<bool>(AuthErrors.ExpiredEmailVerificationToken);
        }
        
        User user = await context.Users.FirstAsync(u => u.Id == verificationToken.UserId, cancellationToken);
        
        user.VerifyEmail(DateTime.UtcNow);
        context.EmailVerificationTokens.Remove(verificationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
