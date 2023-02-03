using Pccl.ProjectTemplate.Application.Dtos;

namespace Pccl.ProjectTemplate.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedDto<UserDto>> GetPagerAsync(UserPagedQueryDto dto);
        Task<UserDto> GetAsync(UserQueryDto dto);
        Task<UserDto> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(Guid id, UserUpdateDto dto);

        #region Permissions
        Task<IEnumerable<PermissionDto>> GetPermissionsAsync(Guid userid);
        #endregion

    }
}
