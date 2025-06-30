using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Invitations;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Invitations.GetById;

internal sealed class GetInvitationByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper): IQueryHandler<GetInvitationByIdQuery, InvitationResponse>
{
    public async Task<Result<InvitationResponse>> Handle(GetInvitationByIdQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;

        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<InvitationResponse>(UserErrors.Unauthorized);
        }

        User? currentUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (currentUser is null)
        {
            return Result.Failure<InvitationResponse>(UserErrors.NotFound(currentUserId));
        }
        
        Invitation invitation = await context.Invitations.AsNoTracking()
            .Include(i => i.Project)
            .Include(i => i.InvitedUser)
            .Include(i => i.InvitedByUser)
            .FirstOrDefaultAsync(i => i.Id == query.Id
                && i.InvitedUserId == currentUserId
                && i.Status == InvitationStatus.Pending, cancellationToken);

        if (invitation is null)
        {
            return Result.Failure<InvitationResponse>(InvitationErrors.NotFound(query.Id.ToString()));
        }
        
        InvitationResponse invitationResponse = mapper.Map<InvitationResponse>(invitation);
        
        return invitationResponse;
    }
}
