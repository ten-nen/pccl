using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pccl.Auth
{
    public interface IPermissionChecker
    {
        Task<bool> CheckPermissionAsync(string userid, IEnumerable<string> roles, string policy, string controller, string action);
        Task<bool> CheckResourcePermissionAsync(string userid, IEnumerable<string> roles, OperationAuthorizationRequirement requirement, IResourceDescriptor resource);
    }
}
