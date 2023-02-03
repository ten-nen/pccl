using Pccl.Audit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pccl.EventBus.AspNetCore
{
    public abstract class BaseHandler<TEvent> : IHandler<TEvent> where TEvent : IEvent
    {
        protected IAuditProvider AuditProvider;
        public BaseHandler()
        {
        }
        public BaseHandler(IAuditProvider auditProvider)
        {
            AuditProvider = auditProvider;
        }
        public virtual async Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                await AddSubscriptionAsync(@event, cancellationToken);
                AuditProvider?.Trace(new HandlerAuditLog(@event, DateTime.Now));
            }
            catch (Exception ex)
            {
                AuditProvider?.Trace(new HandlerAuditLog(@event, DateTime.Now), ex);
            }
        }

        public abstract Task AddSubscriptionAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
