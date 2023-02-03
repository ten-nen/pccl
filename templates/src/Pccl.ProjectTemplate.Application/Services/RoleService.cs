using AutoMapper;
using Pccl.Audit;
using Pccl.AutoDI;
using Pccl.Repository;
using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;

namespace Pccl.ProjectTemplate.Application.Services
{
    public class RoleService : IRoleService, IScopedService
    {
        protected readonly IMapper Mapper;
        protected IUnitOfWork UnitOfWork;
        protected readonly IRoleRepository RoleRepository;
        protected readonly IPermissionRepository PermissionRepository;
        public RoleService(IMapper mapper, IUnitOfWork unitOfWork, IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            RoleRepository = roleRepository;
            PermissionRepository = permissionRepository;
        }

        public virtual async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var list = await RoleRepository.GetAllAsync(true);

            var permissionIds = list.SelectMany(x => x.Permissions.Select(x => x.PermissionId));
            var permissions = await PermissionRepository.GetListByIdsAsync(permissionIds);

            return list.Select(x =>
            {
                var roleDto = Mapper.Map<RoleDto>(x);
                roleDto.RolePermissions = Mapper.Map<List<PermissionDto>>(permissions.Where(p => x.Permissions.Any(rp => rp.PermissionId == p.Id)));
                return roleDto;
            });
        }

        public virtual async Task<RoleDto> CreateAsync(RoleCreateDto dto)
        {
            var exsit = await RoleRepository.ExistsByNameAsync(dto.Name);
            if (exsit)
                throw new BusinessException("角色名称已存在");

            var role = new Role(dto.Name);

            if (dto.RolePermissionNames != null && dto.RolePermissionNames.Any())
            {
                var permissions = await PermissionRepository.GetListByNamesAsync(dto.RolePermissionNames);
                role.SetPermissions(permissions.Select(x => x.Id));
            }

            await RoleRepository.AddAsync(role);
            await UnitOfWork.SaveChangesAsync();
            return Mapper.Map<RoleDto>(role);
        }

        public virtual async Task<RoleDto> UpdateAsync(Guid id, RoleUpdateDto dto)
        {
            var role = await RoleRepository.GetByIdAsync(id, true);
            if (role == null)
                throw new BusinessException("角色不存在");

            var exsit = await RoleRepository.ExistsByNameAsync(dto.Name, id);
            if (exsit)
                throw new BusinessException("角色名称已存在");

            role.SetName(dto.Name);

            if (dto.RolePermissionNames != null && dto.RolePermissionNames.Any())
            {
                var permissions = await PermissionRepository.GetListByNamesAsync(dto.RolePermissionNames);
                role.SetPermissions(permissions.Select(x => x.Id));
            }

            RoleRepository.Update(role);

            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<RoleDto>(role);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var role = await RoleRepository.GetByIdAsync(id);
            if (role == null || role.IsDeleted)
            {
                return;
            }
            RoleRepository.Remove(role);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
