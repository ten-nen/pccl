using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Domain.Interfaces
{

    public interface IPermissionRepository : IRepository<Permission>, IHasCacheStore
    {
        Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Permission>> GetListByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<Permission>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
    }
}
