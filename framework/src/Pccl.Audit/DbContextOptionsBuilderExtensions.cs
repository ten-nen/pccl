using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pccl.Audit
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddAuditInterceptors(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            AddEntityAuditInterceptor(optionsBuilder, serviceProvider);
            AddSqlCommandAuditInterceptor(optionsBuilder, serviceProvider);
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> AddAuditInterceptors<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, IServiceProvider serviceProvider)
            where TContext : DbContext
            => AddAuditInterceptors(optionsBuilder, serviceProvider);

        public static DbContextOptionsBuilder AddEntityAuditInterceptor(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            var auditProvider = serviceProvider.GetRequiredService<IAuditProvider>();
            optionsBuilder.AddInterceptors(new AuditSaveChangesInterceptor(auditProvider));
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> AddEntityAuditInterceptor<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, IServiceProvider serviceProvider)
            where TContext : DbContext
            => AddEntityAuditInterceptor(optionsBuilder,serviceProvider);

        public static DbContextOptionsBuilder AddSqlCommandAuditInterceptor(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            var auditProvider = serviceProvider.GetRequiredService<IAuditProvider>();
            optionsBuilder.AddInterceptors(new AuditDbCommandInterceptor(auditProvider));
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> AddSqlCommandAuditInterceptor<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, IServiceProvider serviceProvider)
            where TContext : DbContext
            => AddSqlCommandAuditInterceptor(optionsBuilder, serviceProvider);
    }
}
