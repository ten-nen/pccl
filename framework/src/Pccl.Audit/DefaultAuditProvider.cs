using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace Pccl.Audit
{
    public class DefaultAuditProvider : IAuditProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger _logger;
        public AuditingOptions Options { get; }
        public string RequestId { get; }
        public DefaultAuditProvider(IHttpContextAccessor httpContextAccessor, IOptions<AuditingOptions> options, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            Options = options.Value;
            _logger = loggerFactory.CreateLogger<DefaultAuditProvider>();
            RequestId = Activity.Current?.Id ?? _httpContextAccessor?.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// for xunit test service inject
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        public DefaultAuditProvider(IOptions<AuditingOptions> options, ILoggerFactory loggerFactory) : this(null, options, loggerFactory)
        {
        }

        public string GetAuditUserClaim(string claimType)
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst(claimType)?.Value;
        }

        public string? GetRequestId()
        {
            return this.RequestId;
        }

        public void Trace<TAuditLog>(TAuditLog[] auditLogs, Exception exception = null) where TAuditLog : AuditLog
        {
            if (auditLogs != null && auditLogs.Length > 0)
            {
                var message = new { AuditLogs = auditLogs, RequestId, UserId = GetAuditUserClaim(JwtClaimTypes.Id) };
                if (exception == null || exception is IBusinessException)
                    _logger.LogInformation("{@message}", message);
                else
                    _logger.LogError(exception, "{@message}", message);
            }
        }

        public void Trace<TAuditLog>(TAuditLog auditLog, Exception exception = null) where TAuditLog : AuditLog
        {
            if (auditLog != null)
            {
                var message = new { AuditLog = auditLog, RequestId, UserId = GetAuditUserClaim(JwtClaimTypes.Id) };
                if (exception == null || exception is IBusinessException)
                    _logger.LogInformation("{@message}", message);
                else
                    _logger.LogError(exception, "{@message}", message);
            }
        }
    }
}
