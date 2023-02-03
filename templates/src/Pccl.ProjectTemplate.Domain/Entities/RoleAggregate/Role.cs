
using System.Collections.ObjectModel;

namespace Pccl.ProjectTemplate.Domain.Entities.RoleAggregate
{
    public class Role : AuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public ICollection<RolePermission> Permissions { get; private set; }

        private Role()
        {
            // required by EF
        }

        public Role(string name)
        {
            Name = name;
            Permissions = new Collection<RolePermission>();
        }

        public virtual void SetName(string name)
        {
            Name = name;
        }

        public virtual bool IsInPermission(Guid permissionId)
        {
            return Permissions.Any(x => x.PermissionId == permissionId);
        }

        public virtual void AddPermission(Guid permissionId)
        {
            if (IsInPermission(permissionId))
                return;
            Permissions.Add(new RolePermission(Id, permissionId));
        }

        public virtual void SetPermissions(IEnumerable<Guid> permissionIds)
        {
            var removes = Permissions.Where(x => !permissionIds.Contains(x.PermissionId));
            foreach (var item in removes)
            {
                Permissions.Remove(item);
            }
            foreach (var permissionId in permissionIds)
            {
                AddPermission(permissionId);
            }
        }
    }
}
