using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Extensions
{
    public static class IdentityModelBuilderExtensions
    {
        public static void ConfigureIdentity(this ModelBuilder modelBuilder)
        {
            const string dbTablePrefix = "Pccl.ProjectTemplate_";
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable(dbTablePrefix + "Users");
                b.Property(x => x.Phone).IsRequired().HasMaxLength(32);
                b.Property(x => x.Password).IsRequired().HasMaxLength(128);
                b.Property(x => x.Name).HasMaxLength(32);
                b.HasMany(x => x.Roles).WithOne().HasForeignKey(x=>x.UserId).IsRequired();
                b.HasIndex(x => x.Phone);

                b.Navigation(x=>x.Roles).UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.ToTable(dbTablePrefix + "UserRoles");
                b.HasIndex(x => x.UserId);
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable(dbTablePrefix + "Roles");
                b.Property(x => x.Name).IsRequired().HasMaxLength(32);
                b.HasMany(x => x.Permissions).WithOne().HasForeignKey(x => x.RoleId).IsRequired();
                b.HasIndex(x => x.Name);
            });

            modelBuilder.Entity<RolePermission>(b =>
            {
                b.ToTable(dbTablePrefix + "RolePermissions");
                b.HasIndex(x => x.RoleId);
            });
        }
    }
}
