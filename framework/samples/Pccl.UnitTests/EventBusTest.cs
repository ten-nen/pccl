using Microsoft.Extensions.DependencyInjection;
using Pccl.EventBus.AspNetCore;

namespace Pccl.UnitTests
{
    public class EventBusTest : TestBase
    {
        [Fact]
        public void TestEventBus()
        {
            var services = new ServiceCollection();
            services.AddEventBus(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton(new ChangeEvent(10));
            WriteServices(services);

            var builder = services.BuildServiceProvider();
            var changeEvent = builder.GetService<ChangeEvent>();
            var eventBusManager = builder.GetService<IEventBusManager>();
            eventBusManager.AddDistributedAsync(changeEvent);

            Assert.Equal(0, changeEvent.Level);
        }
    }


    public class ChangeEvent : BaseEvent, IEvent
    {
        public int Level { get; set; }
        public ChangeEvent(int level) : base(nameof(ChangeEvent))
        {
            Level = level;
        }
    }

    public class ChangeHandler : BaseHandler<ChangeEvent>
    {

        public override Task AddSubscriptionAsync(ChangeEvent @event, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine($"接收到：{@event.EventName},时间:{@event.EventTime},级别:{@event.Level}");
            @event.Level = 0;
            return Task.CompletedTask;
        }
    }
}
