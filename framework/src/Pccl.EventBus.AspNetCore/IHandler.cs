using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Pccl.EventBus.AspNetCore
{
    public interface IHandler<TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
    {
        Task AddSubscriptionAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
