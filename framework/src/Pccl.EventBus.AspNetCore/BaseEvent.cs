using System;

namespace Pccl.EventBus.AspNetCore
{
    public class BaseEvent : IEvent
    {
        public string EventName { get; }

        public DateTime EventTime { get; }
        public BaseEvent(string eventName)
        {
            EventName = eventName;
            EventTime = DateTime.Now;
        }
    }
}
