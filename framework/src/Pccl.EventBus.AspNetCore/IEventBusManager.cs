using System.Threading;
using System.Threading.Tasks;

namespace Pccl.EventBus.AspNetCore
{
    public interface IEventBusManager
    {
        Task AddDistributedAsync(IEvent @event,CancellationToken cancellationToken=default);
    }
}
