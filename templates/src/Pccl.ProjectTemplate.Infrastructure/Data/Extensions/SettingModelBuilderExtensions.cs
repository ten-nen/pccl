using Pccl.ProjectTemplate.Domain.Entities.SettingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Extensions
{
    public static class SettingModelBuilderExtensions
    {
        public static void ConfigureSetting(this ModelBuilder modelBuilder)
        {
            const string dbTablePrefix = "Pccl.ProjectTemplate_";
            modelBuilder.Entity<Setting>(b =>
            {
                b.ToTable(dbTablePrefix + "Settings");
                b.Property(x => x.Name).IsRequired().HasMaxLength(32);
                b.Property(x => x.Value).IsRequired().HasMaxLength(128);
                b.Property(x => x.Description).HasMaxLength(2048);
                b.HasIndex(x => x.Name);
            });
        }
    }
}
