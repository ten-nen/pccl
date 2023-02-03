using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Pccl.Audit
{
    public static class AuditServiceCollectionExtensions
    {
        public static IServiceCollection AddAudit(this IServiceCollection services, Action<AuditingOptions> setOptions = null)
        {
            if (setOptions != null)
                services.Configure(setOptions);
            services.TryAddScoped<IAuditProvider, DefaultAuditProvider>();
            return services;
        }
    }
}
