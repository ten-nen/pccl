using Pccl.Audit;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Domain.Entities
{
    public class AuditedAggregateRoot<TKey> : AuditedEntity<TKey>, IAggregateRoot where TKey : IEquatable<TKey>
    {
    }
}
