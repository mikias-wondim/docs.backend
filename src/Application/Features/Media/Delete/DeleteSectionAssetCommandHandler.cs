using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Files;
using Domain.Media;
using Domain.ProjectMembers;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Media.Delete;

internal sealed class DeleteSectionAssetCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IFileService fileService
) : ICommandHandler<DeleteSectionAssetCommand>
{
    public async Task<Result> Handle(DeleteSectionAssetCommand command, CancellationToken cancellationToken)
    {
        Section? section = await context.Sections
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .Include(s => s.AllowedUsers)
            .FirstOrDefaultAsync(s => s.Id == command.SectionId && s.RecordStatus != RecordStatus.Deleted, cancellationToken);

        if (section is null)
        {
            return Result.Failure(SectionErrors.NotFound(command.SectionId));
        }

        Guid? userId;
        try { userId = userContext.UserId; }
        catch { return Result.Failure(UserErrors.Unauthorized); }

        User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId.Value));
        }

        ProjectRole? role = section.Project.Members.FirstOrDefault(m => m.UserId == userId)?.Role;
        if (!section.IsAccessibleTo(user, role))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        bool deleted = await fileService.DeleteAsync(command.RelativePath);
        return deleted
            ? Result.Success()
            : Result.Failure(MediaAssetErrors.NotFound(command.RelativePath));
    }
}
