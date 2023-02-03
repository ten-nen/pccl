
namespace Pccl.Cache
{
    public interface ICacheStoreProvider
    {
        TCache GetCacheStore<TCache>();
    }
}
