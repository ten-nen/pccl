using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Pccl.Cache
{
    public class DefaultCacheStoreProvider : ICacheStoreProvider
    {
        private IMemoryCache _memoryCache;
        private IDistributedCache _distributedCache;
        public DefaultCacheStoreProvider(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }
        public DefaultCacheStoreProvider(IMemoryCache memoryCache) : this(memoryCache, null)
        {
        }
        public DefaultCacheStoreProvider(IDistributedCache distributedCache) : this(null, distributedCache)
        {
        }

        public TCache GetCacheStore<TCache>()
        {
            if (typeof(IMemoryCache).IsAssignableFrom(typeof(TCache)))
                return (TCache)_memoryCache;
            if (typeof(IDistributedCache).IsAssignableFrom(typeof(TCache)))
                return (TCache)_distributedCache;
            throw new ArgumentException("cache type not found!");
        }
    }
}
