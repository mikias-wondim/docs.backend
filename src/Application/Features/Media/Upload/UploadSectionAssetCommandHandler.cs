using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Files;
using AutoMapper;
using Domain.Media;
using Domain.ProjectMembers;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Media.Upload;

internal sealed class UploadSectionAssetCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IFileService fileService,
    IDateTimeProvider dateTimeProvider,
    IMapper mapper
) : ICommandHandler<UploadSectionAssetCommand, MediaAssetResponse>
{
    private IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;
    public async Task<Result<MediaAssetResponse>> Handle(UploadSectionAssetCommand command, CancellationToken cancellationToken)
    {
        Section? section = await context.Sections
            .Include(s => s.Project)
                .ThenInclude(p => p.Members)
            .Include(s => s.AllowedUsers)
            .FirstOrDefaultAsync(s =>
                s.Id == command.SectionId &&
                s.RecordStatus != RecordStatus.Deleted,
                cancellationToken);

        if (section is null)
        {
            return Result.Failure<MediaAssetResponse>(SectionErrors.NotFound(command.SectionId));
        }

        Guid? currentUserId;
        try { currentUserId = userContext.UserId; }
        catch { return Result.Failure<MediaAssetResponse>(UserErrors.Unauthorized); }

        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<MediaAssetResponse>(UserErrors.NotFound(currentUserId.Value));
        }

        ProjectRole? role = section.Project.Members
            .FirstOrDefault(m => m.UserId == currentUserId)?.Role;

        if (!section.IsAccessibleTo(user, role))
        {
            return Result.Failure<MediaAssetResponse>(UserErrors.Forbidden);
        }

        if (command.File.Length == 0)
        {
            return Result.Failure<MediaAssetResponse>(MediaAssetErrors.EmptyFile);
        }

        string fileName = Guid.NewGuid() + Path.GetExtension(command.File.FileName);
        string directory = $"sections/{section.Id:N}";

        await using Stream stream = command.File.OpenReadStream();
        string relativePath = await fileService.UploadAsync(stream, directory, fileName);

        string uploadedBy = $"{user.FirstName} {user.LastName} ({user.Id})";
        
        var mediaAsset = new MediaAsset(
            id: Guid.NewGuid(),
            name: command.File.FileName,
            altName: command.AltName,
            url: new Uri(relativePath, UriKind.Relative),
            type: command.File.ContentType,
            sectionId: section.Id,
            createdBy: uploadedBy,
            createdAt: DateTimeProvider.GetNow
        );

        context.MediaAssets.Add(mediaAsset);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(mapper.Map<MediaAssetResponse>(mediaAsset));
    }
}
