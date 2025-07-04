using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Pages;
using Domain.Sections;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Sections.Create;

internal sealed class CreateSectionCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateSectionCommand, Guid>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public async Task<Result<Guid>> Handle(CreateSectionCommand command, CancellationToken cancellationToken)
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
        
        User? user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(currentUserId));       
        }

        Project? project = await context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure<Guid>(ProjectErrors.NotFound(command.ProjectId));
        }

        bool isOwner = project.OwnerId == currentUserId;
        bool canWrite = project.Members.Any(m => m.UserId == currentUserId && m.CanWrite());

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
        
        string createBy = $"{user.FirstName} {user.LastName} ({user.Id})";
 
        var section = new Section(
            id: Guid.NewGuid(),
            projectId: project.Id,
            name: command.Name,
            description: command.Description,
            visibility: command.Visibility,
            order: command.Order,
            createdAt: DateTimeProvider.GetNow,
            createdBy: createBy,
            password: command.Password,
            allowedRoles: command.AllowedRoles,
            allowedUserIds: command.AllowedUserIds
        );

        context.Sections.Add(section);

        var defaultPage = new Page(
            id: Guid.NewGuid(),
            sectionId: section.Id,
            title: "New Page",
            order: 0,
            createdBy: createBy,
            createdAt: DateTimeProvider.GetNow
        );

        context.Pages.Add(defaultPage);

        await context.SaveChangesAsync(cancellationToken);

        section.SetDefaultPage(defaultPage.Id);
        context.Sections.Update(section);

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success(section.Id);
    }
}
