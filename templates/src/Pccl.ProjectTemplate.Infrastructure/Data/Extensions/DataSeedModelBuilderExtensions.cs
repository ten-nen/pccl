using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Pccl.Auth;

namespace Pccl.ProjectTemplate.Infrastructure.Data.Extensions
{
    public static class DataSeedModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var now = DateTime.Now;
            var role = new Role("admin") { Id = Guid.Parse("E43BC3ED-312A-4924-A5EE-68190463BAEB") };
            role.SetCreateTime(now);
            modelBuilder.Entity<Role>().HasData(role);

            var adminUser = new User("13333333333", SecurityHasher.HashPassword("123"), "admin") { Id = Guid.Parse("9328F183-5521-43E1-AAD4-341AA9EBE1F5") };
            adminUser.SetCreateTime(now);
            modelBuilder.Entity<User>().HasData(adminUser);

            var userRole = new UserRole(adminUser.Id, role.Id) { Id = Guid.Parse("BE563EFA-9BFE-4E4F-BEB9-0F0F4EF32EEC") };
            modelBuilder.Entity<UserRole>().HasData(userRole);
        }
    }
}
