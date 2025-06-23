using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.UpdateProfile;

internal sealed class UpdateProfileCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext
    ): ICommandHandler<UpdateProfileCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<Guid>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (userContext.UserId != command.UserId)
            {
                return Result.Failure<Guid>(UserErrors.Unauthorized());
            }
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }
        
        User? user = await context.Users
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        Uri? avatarUrl = null;

        if (command.Avatar != null)
        {
            // TODO: Store the file to a storage service and get the stored path
            avatarUrl = new Uri($"https://images.pexels.com/photos/17407385/pexels-photo-17407385.jpeg");
        }
        
        string updatedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        user.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.DisplayName,
            avatarUrl,
            command.Bio,
            updatedBy,
            DateTimeProvider.GetNow);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}
