
using Pccl.Audit;
using Pccl.EventBus.AspNetCore;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate.Events;

namespace Pccl.ProjectTemplate.Application.EventHandlers.UserCreated
{
    public class UserCreatedEventHandler : BaseHandler<UserCreatedEvent>
    {
        public UserCreatedEventHandler(IAuditProvider auditProvider) : base(auditProvider)
        {
        }

        public override Task AddSubscriptionAsync(UserCreatedEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
