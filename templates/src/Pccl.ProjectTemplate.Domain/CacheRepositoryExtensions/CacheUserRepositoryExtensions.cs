using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Pccl.ProjectTemplate.Domain.CacheRepositoryExtensions
{
    public static class CacheUserRepositoryExtensions
    {
        public async static Task<User> GetByIdFromCacheAsync(this IUserRepository userRepository, Guid id)
        {
            var cache = userRepository.CacheStoreProvider?.GetCacheStore<IMemoryCache>();
            var key = string.Format(CacheKeyConsts.User_Id, id);
            if (!cache.TryGetValue<User>(key, out var user))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));
                user = await userRepository.GetAsync(id);
                cache.Set(key, user, cacheEntryOptions);
                //cache.Set(id, user, TimeSpan.FromMinutes(1));
            }
            return user;
        }
    }
}
