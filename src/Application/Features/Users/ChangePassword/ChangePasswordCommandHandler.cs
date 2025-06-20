using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.ChangePassword;

public class ChangePasswordCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider
    ): ICommandHandler<ChangePasswordCommand, bool>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<bool>(UserErrors.Unauthorized());
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<bool>(UserErrors.NotFound(command.UserId));
        }
        
        bool verified = passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<bool>(UserErrors.IncorrectPassword);
        }
        
        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        user.ChangePassword(
            passwordHasher.Hash(command.NewPassword),
            updatedBy,
            DateTimeProvider.GetNow);
        
        bool isUpdated = await context.SaveChangesAsync(cancellationToken) > 0;

        return isUpdated;
    }
}
