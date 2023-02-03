using Pccl.Audit;
using System;

namespace Pccl.EventBus.AspNetCore
{
    public class HandlerAuditLog : AuditLog
    {
        protected IEvent Event { get; set; }
        protected DateTime HandlerTime { get; set; }
        public HandlerAuditLog(IEvent @event, DateTime handlerTime) : base(nameof(EventAuditLog))
        {
            Event = @event;
            HandlerTime = handlerTime;
        }
    }
}
