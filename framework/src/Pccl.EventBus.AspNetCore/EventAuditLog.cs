using Pccl.Audit;

namespace Pccl.EventBus.AspNetCore
{
    public class EventAuditLog : AuditLog
    {
        protected IEvent Event { get; set; }
        public EventAuditLog(IEvent @event) : base(nameof(EventAuditLog))
        {
            Event = @event;
        }
    }
}
