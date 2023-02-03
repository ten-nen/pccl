using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pccl.Repository;
using Pccl.Cache;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Repositories
{
    public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
    {
        public PermissionRepository(DatabaseContext dbContext, ICacheStoreProvider cacheStoreProvider) : base(dbContext, cacheStoreProvider)
        {
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<Permission>().ToListAsync();
        }

        public virtual async Task<IEnumerable<Permission>> GetListByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) => await DbContext.Set<Permission>().Where(x => ids.Contains(x.Id)).ToListAsync();

        public virtual async Task<IEnumerable<Permission>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default) => await DbContext.Set<Permission>().Where(x => names.Contains(x.Name)).ToListAsync();
    }
}
