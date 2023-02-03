

namespace Pccl.ProjectTemplate.Domain.Entities.UserAggregate
{
    public class UserRole : AggregateRoot<Guid>
    {
        public Guid UserId { get;private set; }
        public Guid RoleId { get; private set; }

        private UserRole()
        {
            // required by EF
        }
        public UserRole(Guid userId,Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
