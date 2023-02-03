using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Pccl.Auth
{
    public class ResourcePermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IResourceDescriptor>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IPermissionChecker _permissionChecker;
        public ResourcePermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionChecker = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IPermissionChecker>();
        }
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IResourceDescriptor resource)
        {
            if (context.User.IsInRole("admin"))
            {
                context.Succeed(requirement);
            }
            {
                var id = context.User.FindFirst(JwtClaimTypes.Id)?.Value;
                var roles = context.User.FindAll(JwtClaimTypes.Role).Select(x => x.Value);
                var success = await _permissionChecker.CheckResourcePermissionAsync(id, roles, requirement, resource);
                if (success)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
        }
    }
}
