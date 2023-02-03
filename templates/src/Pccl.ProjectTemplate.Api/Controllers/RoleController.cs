using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pccl.ProjectTemplate.Api.Controllers
{
    public class RoleController : ApiControllerBase
    {
        protected readonly IRoleService RoleService;
        public RoleController(IRoleService roleService)
        {
            RoleService = roleService;
        }

        [HttpGet]
        [Authorize]
        public virtual async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            return await RoleService.GetAllAsync();
        }

        [HttpPost]
        public virtual async Task<RoleDto> CreateAsync([FromBody] RoleCreateDto dto)
        {
            return await RoleService.CreateAsync(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual async Task<RoleDto> UpdateAsync([FromRoute] Guid id, [FromBody] RoleUpdateDto dto)
        {
            return await RoleService.UpdateAsync(id, dto);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync([FromRoute] Guid id)
        {
            await RoleService.DeleteAsync(id);
        }
    }
}
