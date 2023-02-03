using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pccl.Cache
{
    public static class CacheServiceCollectionExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<RedisCacheOptions> setupAction = null)
        {
            services.AddMemoryCache();
            if (setupAction != null)
                services.AddStackExchangeRedisCache(setupAction);

            services.AddSingleton<ICacheStoreProvider, DefaultCacheStoreProvider>();

            return services;
        }
    }
}
