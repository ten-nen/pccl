using Pccl.ProjectTemplate.Application.Dtos;

namespace Pccl.ProjectTemplate.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto> CreateAsync(RoleCreateDto dto);
        Task<RoleDto> UpdateAsync(Guid id, RoleUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
