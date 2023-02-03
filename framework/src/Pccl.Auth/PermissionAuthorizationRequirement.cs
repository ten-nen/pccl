using Microsoft.AspNetCore.Authorization;

namespace Pccl.Auth
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public string PermissionPolicy { get; }
        public PermissionAuthorizationRequirement(string permissionPolicy)
        {
            PermissionPolicy = permissionPolicy;
        }
    }
}
