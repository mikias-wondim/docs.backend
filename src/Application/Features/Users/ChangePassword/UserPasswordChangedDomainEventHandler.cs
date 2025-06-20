using Domain.Users;
using SharedKernel;

namespace Application.Features.Users.ChangePassword;

public class UserPasswordChangedDomainEventHandler: IDomainEventHandler<UserPasswordChangedDomainEvent>
{
    public Task Handle(UserPasswordChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: handle the password change domain event
        return Task.CompletedTask;
    }
}
