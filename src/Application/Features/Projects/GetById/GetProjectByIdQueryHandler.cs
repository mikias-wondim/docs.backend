using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
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
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(
                p => p.Id == query.ProjectId &&
                     (p.Visibility == ProjectVisibility.Public || p.OwnerId == currentUserId), cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.NotFound(query.ProjectId));
        }

        ProjectResponse? response = mapper.Map<ProjectResponse>(project);
        return Result.Success(response);
    }
}
