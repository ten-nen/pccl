using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Pccl.EventBus.AspNetCore
{
    public static class EventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);
            services.AddScoped<IEventBusManager, MediatRPublisher>();
            return services;
        }
    }
}
