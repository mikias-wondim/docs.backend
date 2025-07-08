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

namespace Application.Features.Media.Get;

internal sealed class GetSectionAssetQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IFileService fileService
) : IQueryHandler<GetSectionAssetQuery, Stream>
{
    public async Task<Result<Stream>> Handle(GetSectionAssetQuery query, CancellationToken cancellationToken)
    {
        Section? section = await context.Sections
            .Include(s => s.Project)
            .ThenInclude(p => p.Members)
            .Include(s => s.AllowedUsers)
            .FirstOrDefaultAsync(s => s.Id == query.SectionId && s.RecordStatus != RecordStatus.Deleted, cancellationToken);

        if (section is null)
        {
            return Result.Failure<Stream>(SectionErrors.NotFound(query.SectionId));
        }

        Guid? userId;
        try { userId = userContext.UserId; }
        catch { return Result.Failure<Stream>(UserErrors.Unauthorized); }

        User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Stream>(UserErrors.NotFound(userId.Value));
        }

        ProjectRole? role = section.Project.Members.FirstOrDefault(m => m.UserId == userId)?.Role;
        if (!section.IsAccessibleTo(user, role))
        {
            return Result.Failure<Stream>(UserErrors.Forbidden);
        }

        try
        {
            Stream stream = await fileService.GetAsync(query.RelativePath);
            return Result.Success(stream);
        }
        catch (FileNotFoundException)
        {
            return Result.Failure<Stream>(MediaAssetErrors.NotFound(query.RelativePath));
        }
    }
}
