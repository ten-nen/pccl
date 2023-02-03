using Pccl.Cache;
using Pccl.Repository;
using Pccl.ProjectTemplate.Domain.Entities.SettingAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Repositories
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public DatabaseContext Context { get; set; }
        public SettingRepository(DatabaseContext dbContext, ICacheStoreProvider cacheStoreProvider) : base(dbContext, cacheStoreProvider)
        {
            Context = dbContext;
        }
    }
}
