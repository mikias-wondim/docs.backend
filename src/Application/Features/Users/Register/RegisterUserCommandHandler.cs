using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        const string createdBy = "Guest User";

        var user = new User(
            id: Guid.NewGuid(),
            email: command.Email,
            passwordHash: passwordHasher.Hash(command.Password),
            firstName: command.FirstName,
            lastName: command.LastName,
            createdBy: createdBy,
            timestamp: DateTimeProvider.GetNow
        );

        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
