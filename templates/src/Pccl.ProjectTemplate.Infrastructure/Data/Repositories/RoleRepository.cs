using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pccl.Repository;
using Pccl.Cache;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Repositories
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(DatabaseContext dbContext, ICacheStoreProvider cacheStoreProvider) : base(dbContext, cacheStoreProvider)
        {
        }

        public virtual async Task<IEnumerable<Role>> GetListByIdsAsync(IEnumerable<Guid> ids, bool includePermissions = false, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                return new List<Role>();
            var query = DbContext.Set<Role>().AsQueryable();
            if (includePermissions)
                query = query.Include(x => x.Permissions);
            return await query.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public virtual async Task<IEnumerable<Role>> GetAllAsync(bool includePermissions = false, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<Role>().AsQueryable();
            if (includePermissions)
                query = query.Include(x => x.Permissions);
            return await query.ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? id = null, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<Role>().AsQueryable();
            query = query.Where(x => x.Name.Equals(name));
            if (id.HasValue)
                query = query.Where(x => x.Id != id.Value);
            return await query.AnyAsync();
        }

        public async Task<Role> GetByIdAsync(Guid id, bool includePermissions = false, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<Role>().AsQueryable();
            if (includePermissions)
                query = query.Include(x => x.Permissions);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
