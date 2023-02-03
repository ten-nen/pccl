using System;

namespace Pccl.Audit
{
    public interface IAuditProvider
    {
        AuditingOptions Options { get; }
        string GetAuditUserClaim(string claimType);
        string GetRequestId();
        void Trace<TAuditLog>(TAuditLog[] auditLogs, Exception exception = null) where TAuditLog : AuditLog;
        void Trace<TAuditLog>(TAuditLog auditLog, Exception exception = null) where TAuditLog : AuditLog;
    }
}
