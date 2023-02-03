using System;
using System.Collections.Generic;

namespace Pccl.Audit
{

    public class AuditLog
    {
        public AuditLog(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class EntityHistoryAuditLog : AuditLog
    {
        public EntityHistoryAuditLog(string tableName, string displayName, string schemaName, string primaryKey, string entityState, Dictionary<string, object> oldValues, Dictionary<string, object> newValues) : base(nameof(EntityHistoryAuditLog))
        {
            TableName = tableName;
            DisplayName = displayName;
            SchemaName = schemaName;
            PrimaryKey = primaryKey;
            EntityState = entityState;
            OldValues = oldValues;
            NewValues = newValues;
        }

        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public string PrimaryKey { get; set; }
        public string EntityState { get; set; }
        public Dictionary<string, object> OldValues { get; }
        public Dictionary<string, object> NewValues { get; }
    }

    public class SqlCommandAuditLog : AuditLog
    {
        public SqlCommandAuditLog(int executionDuration, DateTime executionTime, string commandText, string parameters) : base(nameof(SqlCommandAuditLog))
        {
            ExecutionDuration = executionDuration;
            ExecutionTime = executionTime;
            CommandText = commandText;
            Parameters = parameters;
        }
        public double ExecutionDuration { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string CommandText { get; set; }
        public string Parameters { get; set; }
    }

    public class RequestAuditLog : AuditLog
    {
        public RequestAuditLog(int executionDuration, DateTime executionTime, string url, string httpMethod, int? httpStatusCode,  string body, string clientIP, string browserInfo) : base(nameof(RequestAuditLog))
        {
            ExecutionDuration = executionDuration;
            ExecutionTime = executionTime;
            Url = url;
            HttpMethod = httpMethod;
            HttpStatusCode = httpStatusCode;
            Body = body;
            ClientIP = clientIP;
            BrowserInfo = browserInfo;
        }

        public int ExecutionDuration { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public int? HttpStatusCode { get; set; }
        public string Body { get; set; }
        public string ClientIP { get; set; }
        public string BrowserInfo { get; set; }
    }
}
