using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pccl.ProjectTemplate.Api.Controllers
{

    public class PermissionController : ApiControllerBase
    {
        protected readonly IPermissionService PermissionService;
        public PermissionController(IPermissionService permissionService)
        {
            PermissionService = permissionService;
        }

        [HttpGet]
        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            return await PermissionService.GetAllAsync();
        }
    }
}
