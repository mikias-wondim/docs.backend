using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Invitations;
using Domain.Projects;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Projects.GetById;

internal sealed class GetProjectByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
) : IQueryHandler<GetProjectByIdQuery, ProjectResponse>
{
    public async Task<Result<ProjectResponse>> Handle(GetProjectByIdQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            currentUserId = Guid.Empty;
        }

        Project? project = await context.Projects
            .AsNoTracking()
            .Include(p => p.Members)
            .ThenInclude(pm => pm.User)
            .Include(p => p.Owner)
            .Include(p => p.Invitations.Where(i => i.Status == InvitationStatus.Pending))
            .ThenInclude(i => i.InvitedUser)
            .FirstOrDefaultAsync(
                p => p.Id == query.ProjectId &&
                     (p.Visibility == ProjectVisibility.Public || p.OwnerId == currentUserId ||
                      p.Members.Any(pm => pm.UserId == currentUserId)), cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.NotFound(query.ProjectId));
        }

        ProjectResponse? response = mapper.Map<ProjectResponse>(project);
        return Result.Success(response);
    }
}
