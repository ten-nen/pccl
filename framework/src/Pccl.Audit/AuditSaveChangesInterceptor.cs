using IdentityModel;
using Pccl.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Pccl.Audit
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private IAuditProvider _auditProvider;
        public AuditSaveChangesInterceptor(IAuditProvider auditProvider)
        {
            _auditProvider = auditProvider;
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            TryAudit(eventData.Context);
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            TryAudit(eventData.Context);
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            TryAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            TryAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


        private void TryAudit(DbContext context)
        {
            if (_auditProvider?.Options?.IsEnabledEntityAudit != true)
                return;

            //context.ChangeTracker.DetectChanges();
            var auditLogs = new List<AuditLog>();
            foreach (EntityEntry entry in context.ChangeTracker.Entries())
            {
                if (!entry.ShouldBeAudited())
                    continue;

                //设置审计属性
                SetEntriesAuditedProperties(context, entry);

                if (_auditProvider?.Options?.IsEnabledEntityHistoryAudit == true)
                {
                    //版本审计记录
                    var auditLog = CreateAuditLog(entry);
                    auditLogs.Add(auditLog);
                }
            }

            if (_auditProvider?.Options?.IsEnabledEntityHistoryAudit == true && auditLogs.Count > 0)
            {
                _auditProvider.Trace(auditLogs.ToArray());
            }
        }

        private void SetEntriesAuditedProperties(DbContext context, EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                //记录新增属性
                ((IAuditedEntity)entry.Entity).SetCreateTime(DateTime.Now);
                ((IAuditedEntity)entry.Entity).SetCreatedBy(_auditProvider.GetAuditUserClaim(JwtClaimTypes.Id));
            }
            else
            {
                context.Entry((IAuditedEntity)entry.Entity).Property(p => p.CreatedBy).IsModified = false;
                context.Entry((IAuditedEntity)entry.Entity).Property(p => p.CreatedTime).IsModified = false;
            }

            if (entry.State == EntityState.Modified)
            {
                //记录修改属性
                ((IAuditedEntity)entry.Entity).SetModifiedTime(DateTime.Now);
                ((IAuditedEntity)entry.Entity).SetModifiedBy(_auditProvider.GetAuditUserClaim(JwtClaimTypes.Id));

                //记录删除属性
                if (((IAuditedEntity)entry.Entity).IsDeleted)
                {
                    ((IAuditedEntity)entry.Entity).SetDeletedTime(DateTime.Now);
                    ((IAuditedEntity)entry.Entity).SetDeletedBy(_auditProvider.GetAuditUserClaim(JwtClaimTypes.Id));
                }
            }
        }

        private EntityHistoryAuditLog CreateAuditLog(EntityEntry entry)
        {
            var newValues = new Dictionary<string, object>();
            var oldValues = new Dictionary<string, object>();
            foreach (PropertyEntry property in entry.Properties)
            {
                if (property.IsAuditable())
                {
                    if (property.IsTemporary)
                        continue;

                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue;
                                newValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            return new EntityHistoryAuditLog(entry.Metadata.GetTableName(), entry.Metadata.GetSchema(), entry.Metadata.DisplayName(), entry.ToReadablePrimaryKey(), entry.State.ToString(), oldValues, newValues);
        }
    }
}
