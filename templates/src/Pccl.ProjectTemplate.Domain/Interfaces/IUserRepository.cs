using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>, IHasCacheStore
    {
        Task<User> GetAsync(Guid? id = null, string? phone = null, string? password = null, bool includeRoles = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetPagerAsync(string? filter, Guid? roleId, bool? isDeleted, string orderBy = "CreatedTime", int skip = 0, int take = 10, bool includeRoles = false, CancellationToken cancellationToken = default);
        Task<int> GetPagerCountAsync(string? filter, Guid? roleId, bool? isDeleted);
        Task<bool> ExistsByPhoneAsync(string phone, Guid? id = null, CancellationToken cancellationToken = default);
    }
}
