using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider
    ): ICommandHandler<ChangePasswordCommand, bool>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<bool>(UserErrors.Unauthorized);
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<bool>(UserErrors.NotFound(currentUserId));
        }
        
        bool verified = passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<bool>(AuthErrors.IncorrectPassword);
        }
        
        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        user.ChangePassword(
            passwordHasher.Hash(command.NewPassword),
            updatedBy,
            DateTimeProvider.GetNow);
        
        user.Raise(new UserPasswordChangedDomainEvent(user.Id));
        
        bool isUpdated = await context.SaveChangesAsync(cancellationToken) > 0;

        return isUpdated;
    }
}
