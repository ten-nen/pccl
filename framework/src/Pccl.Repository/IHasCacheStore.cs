using Pccl.Cache;

namespace Pccl.Repository
{
    public interface IHasCacheStore
    {
        ICacheStoreProvider CacheStoreProvider { get; }
    }
}
