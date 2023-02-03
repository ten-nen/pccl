using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pccl.ProjectTemplate.Api.Controllers
{
    public class UserController : ApiControllerBase
    {
        protected readonly IUserService UserService;
        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet]
        [Route("Pager")]
        public async Task<PagedDto<UserDto>> GetPagerAsync([FromQuery] UserPagedQueryDto dto)
        {
            return await UserService.GetPagerAsync(dto);
        }

        [HttpPost]
        public async Task<UserDto> CreateAsync([FromBody] UserCreateDto dto)
        {
            return await UserService.CreateAsync(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync([FromRoute] Guid id, [FromBody] UserUpdateDto dto)
        {
            await UserService.UpdateAsync(id, dto);
        }

        [HttpGet]
        [Authorize]
        public virtual async Task<UserDto> GetCurrentAsync()
        {
            return await UserService.GetAsync(new UserQueryDto() { Id = CurrentUser.Id });
        }

        [HttpGet]
        [Route("{id}")]
        public virtual async Task<UserDto> GetAsync([FromRoute] Guid id)
        {
            return await UserService.GetAsync(new UserQueryDto() { Id = id });
        }

        #region Permissions
        [HttpGet]
        [Route("Permissions")]
        [Authorize]
        public virtual async Task<IEnumerable<PermissionDto>> GetCurrentPermissionsAsync()
        {
            if (CurrentUser.Roles == null || CurrentUser.Roles.Count <= 0)
                return new List<PermissionDto>();
            return await UserService.GetPermissionsAsync(CurrentUser.Id);
        }
        #endregion
    }
}
