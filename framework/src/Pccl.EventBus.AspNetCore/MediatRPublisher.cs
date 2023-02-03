using Pccl.Audit;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Pccl.EventBus.AspNetCore
{
    public class MediatRPublisher : IEventBusManager
    {
        protected IMediator Mediator;
        protected IAuditProvider AuditProvider;
        public MediatRPublisher(IMediator mediator)
        {
            Mediator = mediator;
        }
        public MediatRPublisher(IMediator mediator, IAuditProvider auditProvider)
        {
            Mediator = mediator;
            AuditProvider = auditProvider;
        }
        public async Task AddDistributedAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                await Mediator.Publish(@event, cancellationToken);
                AuditProvider?.Trace(new EventAuditLog(@event));
            }
            catch (Exception ex)
            {
                AuditProvider?.Trace(new EventAuditLog(@event), ex);
            }
        }
    }
}
