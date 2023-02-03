using Pccl.ProjectTemplate.Application.Dtos;

namespace Pccl.ProjectTemplate.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
    }
}
