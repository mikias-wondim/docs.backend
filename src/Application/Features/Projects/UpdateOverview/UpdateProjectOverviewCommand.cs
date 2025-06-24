using Application.Abstractions.Messaging;

namespace Application.Features.Projects.UpdateOverview;

public record UpdateProjectOverviewCommand(Guid ProjectId, string Overview): ICommand<Guid>;
