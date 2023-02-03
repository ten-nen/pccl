
namespace Pccl.Audit
{
    public class AuditingOptions
    {
        /// <summary>
        /// 启用实体审计
        /// </summary>
        public bool IsEnabledEntityAudit { get; set; } = true;
        /// <summary>
        /// 启用实体历史审计
        /// </summary>
        public bool IsEnabledEntityHistoryAudit { get; set; } = true;
        /// <summary>
        /// 启用SQL审计
        /// </summary>
        public bool IsEnabledSqlCommandAudit { get; set; } = true;
        /// <summary>
        /// 大于N毫秒SQL审计
        /// </summary>
        public double SqlCommandAuditMinExecutionDuration { get; set; }
        /// <summary>
        /// 启用请求审计
        /// </summary>
        public bool IsEnabledRequestAudit { get; set; } = true;
        /// <summary>
        /// 大于N毫秒请求审计
        /// </summary>
        public double RequestAuditMinExecutionDuration { get; set; }
    }
}
