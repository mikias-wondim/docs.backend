using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Sections.Update;

internal sealed class UpdateSectionCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider): ICommandHandler<UpdateSectionCommand>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result> Handle(UpdateSectionCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized);
        }
        
        Section? section = await context.Sections
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(s => s.Id == command.SectionId, cancellationToken);

        if (section is null)
        {
            return Result.Failure<Guid>(SectionErrors.NotFound(command.SectionId));
        }

        bool isOwner = section.Project.OwnerId == currentUserId;
        bool canWrite = section.Project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }
        
        if (command.AllowedUserIds is not null && command.AllowedUserIds.Count > 0)
        {
            int users = await context.Users
                .Where(u => command.AllowedUserIds.Contains(u.Id))
                .CountAsync(cancellationToken);

            if (users != command.AllowedUserIds.Count)
            {
                return Result.Failure<Guid>(SectionErrors.AllowedUsersNotFound);
            }
        }
        
        if (command.AllowedUserIds is not null && command.AllowedUserIds.Count > 0)
        {
            int found = await context.Users
                .Where(u => command.AllowedUserIds.Contains(u.Id))
                .CountAsync(cancellationToken);

            if (found != command.AllowedUserIds.Count)
            {
                return Result.Failure(SectionErrors.AllowedUsersNotFound);
            }
        }

        // Perform the update
        section.Update(
            command.Name,
            command.Description,
            command.Visibility,
            command.Password,
            command.AllowedRoles,
            command.AllowedUserIds ?? [],
            currentUserId.ToString(),
            DateTimeProvider.UtcNow
        );

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
