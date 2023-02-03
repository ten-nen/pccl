using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pccl.Repository;
using Pccl.Cache;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(DatabaseContext dbContext, ICacheStoreProvider cacheStoreProvider) : base(dbContext, cacheStoreProvider)
        {

        }

        public async Task<IEnumerable<User>> GetPagerAsync(string? filter, Guid? roleId, bool? isDeleted, string orderBy = "CreatedTime", int skip = 0, int take = 20, bool includeRoles = false, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<User>().AsQueryable();
            if (includeRoles)
                query = query.Include(x => x.Roles);
            if (!string.IsNullOrWhiteSpace(filter))
                query.Where(x => (x.Phone != null && x.Phone.Contains(filter)) || (x.Name != null && x.Name.Contains(filter)));
            if (isDeleted.HasValue)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            if (roleId.HasValue)
                query = query.Where(x => x.Roles.Any(r => r.RoleId == roleId.Value));
            if (orderBy != null)
                switch (orderBy.ToLower())
                {
                    case "createdtime desc":
                        query = query.OrderByDescending(x => x.CreatedTime);
                        break;
                    case "modifiedtime":
                        query = query.OrderBy(x => x.ModifiedTime);
                        break;
                    case "modifiedtime desc":
                        query = query.OrderByDescending(x => x.ModifiedTime);
                        break;
                    default:
                        query = query.OrderBy(x => x.CreatedTime);
                        break;
                }

            return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<int> GetPagerCountAsync(string? filter, Guid? roleId, bool? isDeleted)
        {
            var query = DbContext.Set<User>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
                query.Where(x => (x.Phone != null && x.Phone.Contains(filter)) || (x.Name != null && x.Name.Contains(filter)));
            if (isDeleted.HasValue)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            if (roleId.HasValue)
                query = query.Where(x => x.Roles.Any(r => r.RoleId == roleId.Value));
            return await query.CountAsync();
        }

        public async Task<User> GetAsync(Guid? id = null, string? phone = null, string? password = null, bool includeRoles = false, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<User>().AsQueryable();
            if (includeRoles)
                query = query.Include(x => x.Roles);
            if (id.HasValue)
                query = query.Where(x => x.Id == id);
            if (phone != null)
                query = query.Where(x => x.Phone == phone);
            if (password != null)
                query = query.Where(x => x.Password == password);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, Guid? id = null, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<User>().AsQueryable();
            query = query.Where(x => x.Phone.Equals(phone));
            if (id.HasValue)
                query = query.Where(x => x.Id != id.Value);
            return await query.AnyAsync();
        }
    }
}
