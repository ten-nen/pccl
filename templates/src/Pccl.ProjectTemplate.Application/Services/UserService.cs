using AutoMapper;
using Pccl.Audit;
using Pccl.Auth;
using Pccl.AutoDI;
using Pccl.EventBus.AspNetCore;
using Pccl.Repository;
using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.ProjectTemplate.Domain.CacheRepositoryExtensions;
using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Pccl.ProjectTemplate.Domain.Interfaces;

namespace Pccl.ProjectTemplate.Application.Services
{
    public class UserService : IUserService, IScopedService
    {
        protected readonly IMapper Mapper;
        protected IUnitOfWork UnitOfWork;
        protected IEventBusManager EventBusManager;
        protected readonly IUserRepository UserRepository;
        protected readonly IRoleRepository RoleRepository;
        protected readonly IPermissionRepository PermissionRepository;
        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IEventBusManager eventBusManager, IUserRepository userRepository, IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            EventBusManager = eventBusManager;
            UserRepository = userRepository;
            RoleRepository = roleRepository;
            PermissionRepository = permissionRepository;
        }

        public virtual async Task<PagedDto<UserDto>> GetPagerAsync(UserPagedQueryDto dto)
        {
            var count = await UserRepository.GetPagerCountAsync(dto.Filter, dto.RoleId, false);
            var users = await UserRepository.GetPagerAsync(dto.Filter, dto.RoleId, false, dto.Sorting, dto.PageSize * (dto.PageIndex - 1), dto.PageSize, true);
            var roles = await RoleRepository.GetListByIdsAsync(users.SelectMany(x => x.Roles.Select(r => r.RoleId)));

            var userDtos = users.Select(x =>
            {
                var userDto = Mapper.Map<UserDto>(x);
                userDto.UserRoles = Mapper.Map<List<RoleDto>>(roles.Where(r => x.Roles.Any(ur => ur.RoleId == r.Id)));
                return userDto;
            });
            return new PagedDto<UserDto>(userDtos, count);
        }


        public virtual async Task<UserDto> GetAsync(UserQueryDto dto)
        {
            var user = await UserRepository.GetAsync(dto.Id, dto.Phone, dto.Password, true);
            if (user == null)
                return null;

            var cacheUser = await UserRepository.GetByIdFromCacheAsync(user.Id);

            var roles = await RoleRepository.GetListByIdsAsync(user.Roles.Select(x => x.RoleId));
            var r = Mapper.Map<UserDto>(user);
            r.UserRoles = Mapper.Map<List<RoleDto>>(roles);
            return r;
        }

        public virtual async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            var exsit = await UserRepository.ExistsByPhoneAsync(dto.Phone);
            if (exsit)
                throw new BusinessException("手机号已存在");

            var user = new User(dto.Phone, SecurityHasher.HashPassword(dto.Password), dto.Name);

            if (dto.UserRoleIds != null)
            {
                var roles = await RoleRepository.GetListByIdsAsync(dto.UserRoleIds);
                user.SetRoles(roles.Select(x => x.Id));
            }

            await UserRepository.AddAsync(user);
            await UnitOfWork.SaveChangesAsync();
            return Mapper.Map<UserDto>(user);
        }

        public virtual async Task UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var user = await UserRepository.GetAsync(id, includeRoles: true);
            if (user == null)
                throw new BusinessException("用户不存在");

            var exsit = await UserRepository.ExistsByPhoneAsync(dto.Phone, id);
            if (exsit)
                throw new BusinessException("手机号已存在");

            user.SetPhone(dto.Phone);
            user.SetName(dto.Name);
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.SetPassword(SecurityHasher.HashPassword(dto.Password));

            if (dto.UserRoleIds != null)
            {
                var roles = await RoleRepository.GetListByIdsAsync(dto.UserRoleIds);
                user.SetRoles(roles.Select(x => x.Id));
            }


            UserRepository.Update(user);

            await UnitOfWork.SaveChangesAsync();
        }

        #region Permissions
        public virtual async Task<IEnumerable<PermissionDto>> GetPermissionsAsync(Guid userid)
        {
            var user = await UserRepository.GetAsync(userid, null, null, true);
            var roles = await RoleRepository.GetListByIdsAsync(user.Roles.Select(x => x.RoleId), true);
            if (roles.Count() <= 0)
                return new List<PermissionDto>();
            IEnumerable<Permission> permissions = null;
            if (roles.Any(x => x.Name == "admin"))
                permissions = await PermissionRepository.GetAllAsync();
            else
                permissions = await PermissionRepository.GetListByIdsAsync(roles.SelectMany(x => x.Permissions.Select(p => p.PermissionId)));
            return Mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }
        #endregion
    }
}
