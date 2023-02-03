using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Microsoft.EntityFrameworkCore;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Extensions
{
    public static class PermissionModelBuilderExtensions
    {
        public static void ConfigurePermission(this ModelBuilder modelBuilder)
        {
            const string dbTablePrefix = "Pccl.ProjectTemplate_";
            modelBuilder.Entity<Permission>(b =>
            {
                b.ToTable(dbTablePrefix + "Permissions");
                b.Property(x => x.Name).IsRequired().HasMaxLength(128).IsUnicode(false);
            });
        }
    }
}
