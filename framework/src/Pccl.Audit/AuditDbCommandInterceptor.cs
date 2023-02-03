using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pccl.Audit
{
    public class AuditDbCommandInterceptor : DbCommandInterceptor
    {
        private IAuditProvider _auditProvider;
        public AuditDbCommandInterceptor(IAuditProvider auditProvider)
        {
            _auditProvider = auditProvider;
        }
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            CommandExecuted(command, eventData);
            base.CommandFailed(command, eventData);
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            CommandExecuted(command, eventData);
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            CommandExecuted(command, eventData);
            return base.NonQueryExecuted(command, eventData, result);
        }
        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            CommandExecuted(command, eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            CommandExecuted(command, eventData);
            return base.ReaderExecuted(command, eventData, result);
        }
        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            CommandExecuted(command, eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
        public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
        {
            CommandExecuted(command, eventData);
            return base.ScalarExecuted(command, eventData, result);
        }
        public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
        {
            CommandExecuted(command, eventData);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }
        private void CommandExecuted(DbCommand command, CommandEventData eventData)
        {
            if (_auditProvider?.Options?.IsEnabledSqlCommandAudit != true)
                return;

            var parameters = new StringBuilder();
            foreach (DbParameter param in command.Parameters)
            {
                parameters.AppendLine(param.ParameterName + " " + param.DbType + " = " + param.Value);
            }

            Exception exception = null;
            double executionDuration = 0d;
            if (eventData is CommandErrorEventData)
            {
                var d = ((CommandErrorEventData)eventData);
                exception = d.Exception;
                executionDuration = d.Duration.TotalMilliseconds;
            }
            else if (eventData is CommandExecutedEventData)
            {
                var d = ((CommandExecutedEventData)eventData);
                executionDuration = d.Duration.TotalMilliseconds;
            }

            if (exception == null && executionDuration < _auditProvider.Options.SqlCommandAuditMinExecutionDuration)
                return;

            var auditLog = new SqlCommandAuditLog((int)executionDuration, eventData.StartTime.UtcDateTime, command.CommandText, parameters.ToString());
            _auditProvider.Trace(auditLog, exception);
        }

    }
}
