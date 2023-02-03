using System;

namespace Pccl.Audit
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}
