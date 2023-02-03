using Pccl.EventBus.AspNetCore;

namespace Pccl.ProjectTemplate.Domain.Entities.UserAggregate.Events
{
    public class UserCreatedEvent : BaseEvent, IEvent
    {
        public User User { get; set; }
        public UserCreatedEvent(User user):base(nameof(UserCreatedEvent))
        {
            User = user;
        }
    }
}
