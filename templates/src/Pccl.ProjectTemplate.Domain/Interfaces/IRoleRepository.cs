using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<Role>> GetListByIdsAsync(IEnumerable<Guid> ids, bool includePermissions = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetAllAsync(bool includePermissions = false, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name , Guid? id = null, CancellationToken cancellationToken = default);
        Task<Role> GetByIdAsync(Guid id, bool includePermissions = false, CancellationToken cancellationToken = default);
    }
}
