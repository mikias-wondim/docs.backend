using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Pages;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Faqs.GetByPageId;

internal sealed class GetFaqsByPageIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IMapper mapper
    ): IQueryHandler<GetFaqsByPageIdQuery, List<FaqResponse>>
{
    public async Task<Result<List<FaqResponse>>> Handle(GetFaqsByPageIdQuery query, CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = userContext.UserId;
        }
        catch (ApplicationException)
        {
            return Result.Failure<List<FaqResponse>>(UserErrors.Unauthorized);
        }

        User? user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<FaqResponse>>(UserErrors.NotFound(currentUserId));
        }

        Page? page = await context.Pages
            .AsNoTracking()
            .Include(p => p.Section)
            .ThenInclude(s => s.Project)
            .ThenInclude(s => s.Members)
            .Include(p => p.Faqs)
            .FirstOrDefaultAsync(p => p.Id == query.PageId, cancellationToken);

        if (page is null)
        {
            return Result.Failure<List<FaqResponse>>(PageErrors.NotFound(query.PageId));
        }
        
        bool isOwner = page.Section.Project.OwnerId == currentUserId;
        bool canWrite = page.Section.Project.Members
            .Any(m => m.UserId == currentUserId && m.CanWrite());

        if (!isOwner && !canWrite)
        {
            return Result.Failure<List<FaqResponse>>(UserErrors.Forbidden);
        }
        
        List<FaqResponse> faqResponses = [.. page.Faqs
            .OrderBy(f => f.Order)
            .Select(mapper.Map<FaqResponse>)];
        
        return Result.Success(faqResponses);
    }
}
