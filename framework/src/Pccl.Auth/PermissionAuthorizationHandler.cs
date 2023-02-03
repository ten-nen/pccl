using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pccl.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IConfiguration _configuration;
        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _permissionChecker = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IPermissionChecker>();
        }
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {

            }
            else if (context.User.IsInRole("admin"))
            {
                context.Succeed(requirement);
            }
            else
            {
                var ignoreAuthorize = _httpContextAccessor.HttpContext.GetEndpoint().Metadata.Any(x => x is AuthorizeAttribute && ((AuthorizeAttribute)x).Policy == null);
                if (ignoreAuthorize)
                {
                    context.Succeed(requirement);
                    return;
                }

                var id = context.User.FindFirst(JwtClaimTypes.Id)?.Value;
                var roles = context.User.FindAll(JwtClaimTypes.Role).Select(x => x.Value);

                var success = false;
                var permissions = new List<string>();
                var policies = _configuration.GetSection("App:AuthPolicies").GetChildren().Select(x=>x.Value);
                var policy = policies.FirstOrDefault(x => x.Equals(requirement.PermissionPolicy, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(policy))
                {
                    var controller = Convert.ToString(_httpContextAccessor.HttpContext.Request.RouteValues["controller"]);
                    var action = Convert.ToString(_httpContextAccessor.HttpContext.Request.RouteValues["action"]);
                    success = await _permissionChecker.CheckPermissionAsync(id, roles, policy, controller, action);
                }
                if (success)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
        }
    }
}
