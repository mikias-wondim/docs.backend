using Domain.Projects;
using SharedKernel;

namespace Application.Features.Projects.Create;

public class ProjectCreatedDomainEventHandler: IDomainEventHandler<ProjectCreatedDomainEvent>
{
    public Task Handle(ProjectCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Handle project created even, etc.
        return Task.CompletedTask;
    }
}
