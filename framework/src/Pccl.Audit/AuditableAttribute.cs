using System;
namespace Pccl.Audit
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AuditableAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class NonAuditableAttribute : Attribute
    { }
}
