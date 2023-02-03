using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Pccl.Auth
{
    public class RoutePermissionDefinitionProvider : IPermissionDefinitionProvider
    {
        private readonly IActionDescriptorCollectionProvider  _actionDescriptorCollectionProvider;
        private readonly IConfiguration _configuration;
        public RoutePermissionDefinitionProvider(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IConfiguration configuration)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _configuration = configuration;
        }
        public List<string> GetAllDefinitions()
        {
            var permissions = new List<string>();
            var policies = _configuration.GetSection("App:AuthPolicies").GetChildren().Select(x => x.Value);
            if (policies.Count() == 0) return permissions;

            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                                                    .Where(x => !x.EndpointMetadata.Any(e => e is AllowAnonymousAttribute) && x.EndpointMetadata.Any(e => e is AuthorizeAttribute))
                                                    .Select(x => new
                                                    {
                                                        Action = x.RouteValues["Action"],
                                                        Controller = x.RouteValues["Controller"],
                                                        Policy = ((AuthorizeAttribute)x.EndpointMetadata.FirstOrDefault(e => e is AuthorizeAttribute))?.Policy
                                                    })
                                                    .ToList();
            foreach (var p in policies)
            {
                var policyRoutes = routes.Where(v => v.Policy == p).ToList();
                foreach (var controller in policyRoutes.Select(v => v.Controller).Distinct())
                {
                    permissions.Add($"{p.ToLower()}.{controller.ToLower()}");
                    foreach (var action in policyRoutes.Where(v => v.Controller == controller).Select(v => v.Action).Distinct())
                    {
                        permissions.Add($"{p.ToLower()}.{controller.ToLower()}.{action.ToLower()}");
                    }
                }
            }
            return permissions;
        }
    }
}