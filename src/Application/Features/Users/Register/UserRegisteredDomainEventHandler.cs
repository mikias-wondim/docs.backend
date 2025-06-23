using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Register;

public sealed class UserRegisteredDomainEventHandler(
    IApplicationDbContext context,
    IEmailVerificationService emailVerificationService,
    ITokenProvider tokenProvider) : IDomainEventHandler<UserRegisteredDomainEvent>
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

        context.EmailVerificationTokens.Add(emailVerification);
        await context.SaveChangesAsync(cancellationToken);

        await emailVerificationService.SendVerificationEmailAsync(
            emailVerification.Token,
            user.Email,
            cancellationToken
        );
    }
}
