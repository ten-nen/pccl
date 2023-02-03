using MediatR;
using System;

namespace Pccl.EventBus.AspNetCore
{
    public interface IEvent: INotification
    {
        public string EventName { get; }
        public DateTime EventTime { get;}
    }
}