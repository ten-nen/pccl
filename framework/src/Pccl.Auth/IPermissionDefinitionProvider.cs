using System.Collections.Generic;

namespace Pccl.Auth
{
    public interface IPermissionDefinitionProvider
    {
        List<string> GetAllDefinitions();
    }
}
