using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Faqs.GetByPageId;

public sealed record GetFaqsByPageIdQuery(
    Guid PageId
) : IQuery<List<FaqResponse>>;
