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
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

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
