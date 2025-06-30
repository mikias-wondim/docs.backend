using Application.Abstractions.Messaging;

namespace Application.Features.Users.SearchByProjectId;

public sealed record SearchUsersByProjectIdQuery(Guid ProjectId, string Query): IQuery<List<UserSummaryResponse>>;
