using Application.Abstractions.Messaging;

namespace Application.Features.Projects.GetById;

public record GetProjectByIdQuery(Guid ProjectId): IQuery<ProjectResponse>;
