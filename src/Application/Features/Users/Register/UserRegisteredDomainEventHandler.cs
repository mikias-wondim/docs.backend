using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Services.Email;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Features.Users.Register;

public sealed class UserRegisteredDomainEventHandler(
    IApplicationDbContext context,
    IEmailVerificationService emailVerificationService,
    ITokenProvider tokenProvider,
    ILogger<UserRegisteredDomainEventHandler> logger
    ) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        User user = await context.Users.FirstOrDefaultAsync(u => u.Id == domainEvent.UserId, cancellationToken);

        if (user is null || user.EmailVerified)
        {
            return;
        }

        var emailVerification = new EmailVerificationToken(
            Guid.NewGuid(),
            user.Id,
            tokenProvider.GenerateRandomToken(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2)
        );
        
        try
        {
            context.EmailVerificationTokens.Add(emailVerification);
            await context.SaveChangesAsync(cancellationToken);
            
            await emailVerificationService.SendAsync(
                emailVerification.Token,
                user.Email,
                cancellationToken
            );

            logger.LogInformation("Verification email sent to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to send email verification to {Email}. Token: {Token}",
                user.Email,
                emailVerification.Token);
        }
    }
}
