using Microsoft.Extensions.DependencyInjection;
using Pccl.AutoDI;
using System.Reflection;

namespace Pccl.BackgroundWorker
{
    public static class BackgroundWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddBackgroundWorker(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies != null)
                services.AddServices<IWork>(ServiceLifetime.Singleton, assemblies: assemblies);
            return services;
        }
    }
}
