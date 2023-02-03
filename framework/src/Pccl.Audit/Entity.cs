using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pccl.Audit
{
    [Serializable]
    public abstract class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public virtual TKey Id { get; set; }
    }
}
