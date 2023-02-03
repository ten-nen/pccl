
namespace Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate
{
    public class Permission : AggregateRoot<Guid>
    {
        public string Name { get; private set; }

        private Permission()
        {
            // required by EF
        }
        public Permission(string name)
        {
            Name= name;
        }
    }
}