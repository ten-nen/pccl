using Pccl.AutoDI;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Pccl.Repository
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddServices<IUnitOfWork>(ServiceLifetime.Scoped, (serviceProvider, implementationType) => serviceProvider.GetService(implementationType), assemblies: assemblies);

            services.AddServices(typeof(IRepository<>), ServiceLifetime.Scoped, assemblies: assemblies);

            return services;
        }
    }
}
