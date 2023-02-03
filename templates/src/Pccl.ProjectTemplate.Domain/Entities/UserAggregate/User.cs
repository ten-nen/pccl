using System.Collections.ObjectModel;

namespace Pccl.ProjectTemplate.Domain.Entities.UserAggregate
{
    public class User : AuditedAggregateRoot<Guid>
    {
        public string Phone { get; private set; }
        public string Password { get; private set; }
        public string? Name { get; private set; }
        public DateTimeOffset? LockoutEnd { get; private set; }

        public ICollection<UserRole> Roles { get; private set; }

        private User()
        {
            // required by EF
        }

        public User(string phone, string password, string name = null)
        {
            Phone = phone;
            Password = password;
            Name = name;
            Roles = new Collection<UserRole>();
        }
        public virtual bool IsInRole(Guid roleId)
        {
            return Roles.Any(r => r.RoleId == roleId);
        }

        public virtual void AddRole(Guid RoleId)
        {
            if (IsInRole(RoleId))
                return;
            Roles.Add(new UserRole(Id, RoleId));
        }

        public virtual void SetRoles(IEnumerable<Guid> roleIds)
        {
            var removes = Roles.Where(x => !roleIds.Contains(x.RoleId));
            foreach (var item in removes)
            {
                Roles.Remove(item);
            }
            foreach (var roleId in roleIds)
            {
                AddRole(roleId);
            }
        }

        public virtual void SetPhone(string phone)
        {
            Phone = phone;
        }

        public virtual void SetName(string? name)
        {
            Name = name;
        }

        public virtual void SetPassword(string password)
        {
            Password = password;
        }
    }
}
