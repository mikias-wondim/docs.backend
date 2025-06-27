using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.GetOwn;

internal sealed class GetOwnQueryHandler(IApplicationDbContext context, IUserContext userContext, IMapper mapper)
    : IQueryHandler<GetOwnQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetOwnQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized);
        }
        
        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(currentUserId));
        }

        UserResponse response = mapper.Map<UserResponse>(user);
        
        return response;
    }
}
