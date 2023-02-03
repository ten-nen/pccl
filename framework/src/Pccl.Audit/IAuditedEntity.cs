using System;

namespace Pccl.Audit
{
    public interface IAuditedEntity
    {
        DateTime CreatedTime { get; }

        string CreatedBy { get; }

        DateTime? ModifiedTime { get; }

        string ModifiedBy { get; }

        bool IsDeleted { get; }

        string DeletedBy { get; }

        DateTime? DeletedTime { get; }

        void SetCreateTime(DateTime createTime);
        void SetCreatedBy(string createdBy);
        void SetModifiedBy(string modifiedBy);
        void SetModifiedTime(DateTime? modifiedTime);
        void SetIsDeleted(bool isDeleted);
        void SetDeletedBy(string deletedBy);
        void SetDeletedTime(DateTime? deletedTime);

    }
}
