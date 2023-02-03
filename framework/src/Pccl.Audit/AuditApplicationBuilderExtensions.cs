using Microsoft.AspNetCore.Builder;

namespace Pccl.Audit
{
    public static class AuditApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAudit(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditMiddleware>();
        }
    }
}
