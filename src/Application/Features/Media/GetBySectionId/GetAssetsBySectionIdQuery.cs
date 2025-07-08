using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Media.GetBySectionId;

public sealed record GetAssetsSectionByIdQuery(
    Guid SectionId,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 10) 
    : IQuery<PagedResult<MediaAssetResponse>>;
