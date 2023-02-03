using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.Auth;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Pccl.ProjectTemplate.Api
{
    public class ApiPermissionChecker : IPermissionChecker
    {
        protected readonly IRoleService RoleService;
        protected readonly IPermissionService PermissionService;
        public ApiPermissionChecker(IRoleService roleService,IPermissionService permissionService)
        {
            RoleService = roleService;
            PermissionService = permissionService;
        }
        public async Task<bool> CheckPermissionAsync(string userid, IEnumerable<string> roles, string policy, string controller, string action)
        {
            if (roles.Contains("admin"))
                return true;
            var all = await RoleService.GetAllAsync();
            return all.Where(x => roles.Contains(x.Name)).Any(x => x.RolePermissions.Any(x => x.Name == $"{policy.ToLower()}.{controller.ToLower()}.{action.ToLower()}"));
        }

        public async Task<bool> CheckResourcePermissionAsync(string userid, IEnumerable<string> roles, OperationAuthorizationRequirement requirement, IResourceDescriptor resource)
        {
            if (roles.Contains("admin"))
                return true;
            var all = await PermissionService.GetAllAsync();
            return false;
        }
    }
}
