using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.GetById;

internal sealed class GetUserByIdQueryHandler(IApplicationDbContext context, IUserContext userContext, IMapper mapper)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public IUserContext UserContext { get; } = userContext;

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        // TODO: Check if the current user has the permission to view other users

        User? user = await context.Users
            .Where(u => u.Id == query.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        UserResponse response = mapper.Map<UserResponse>(user);
        
        return response ?? Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
    }
}
