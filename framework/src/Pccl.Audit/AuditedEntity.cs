using System;
using System.ComponentModel.DataAnnotations;

namespace Pccl.Audit
{
    [Serializable]
    public abstract class AuditedEntity<TKey> : Entity<TKey>, IAuditedEntity where TKey : IEquatable<TKey>
    {
        public virtual DateTime CreatedTime { get; private set; }

        [StringLength(36)]
        public virtual string CreatedBy { get; private set; }

        public virtual DateTime? ModifiedTime { get; private set; }

        [StringLength(36)]
        public virtual string ModifiedBy { get; private set; }

        [Required]
        public virtual bool IsDeleted { get; private set; }

        [StringLength(36)]
        public virtual string DeletedBy { get; private set; }

        public virtual DateTime? DeletedTime { get; private set; }

        public virtual void SetCreatedBy(string createdBy)
        {
            CreatedBy = createdBy;
        }

        public virtual void SetCreateTime(DateTime createTime)
        {
            CreatedTime = createTime;
        }

        public virtual void SetDeletedBy(string deletedBy)
        {
            DeletedBy = deletedBy;
        }

        public virtual void SetDeletedTime(DateTime? deletedTime)
        {
            DeletedTime = deletedTime;
        }

        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }

        public virtual void SetModifiedBy(string modifiedBy)
        {
            ModifiedBy = modifiedBy;
        }

        public virtual void SetModifiedTime(DateTime? modifiedTime)
        {
            ModifiedTime = modifiedTime;
        }
    }
}
