using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Entities.SettingAggregate;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Pccl.ProjectTemplate.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Pccl.Repository;

namespace Pccl.ProjectTemplate.Infrastructure.Data
{
    public class DatabaseContext : DbContext, IUnitOfWork
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public void BeginTransaction() => Database.BeginTransaction();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => await Database.BeginTransactionAsync(cancellationToken);

        public void CommitTransaction() => Database.CommitTransaction();

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await Database.CommitTransactionAsync(cancellationToken);

        public void RollbackTransaction() => Database.RollbackTransaction();

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await Database.RollbackTransactionAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //todo: 1、创建数据库 dotnet ef migrations add InitialCreate -o ./Data/Migrations --verbose
            //                 dotnet ef database update
            
            base.OnModelCreating(builder);

            builder.ConfigureIdentity();
            builder.ConfigurePermission();
            builder.ConfigureSetting();

            builder.Seed();
        }
    }
}
