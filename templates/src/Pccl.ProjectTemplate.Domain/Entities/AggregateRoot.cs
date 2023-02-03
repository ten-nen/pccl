using Pccl.Audit;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Domain.Entities
{
    public class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot where TKey : IEquatable<TKey>
    {
    }
}
