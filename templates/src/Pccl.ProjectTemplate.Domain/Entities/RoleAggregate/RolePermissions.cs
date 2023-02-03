using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pccl.ProjectTemplate.Domain.Entities.RoleAggregate
{
    public class RolePermission : AuditedAggregateRoot<Guid>
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        private RolePermission()
        {
            // required by EF
        }
        public RolePermission(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
