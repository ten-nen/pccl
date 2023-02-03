using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pccl.Auth;

namespace Pccl.ProjectTemplate.Infrastructure.Data
{
    public class DatabaseContextSeed
    {
        public static async Task SeedRoutePermissionsAsync(IPermissionDefinitionProvider permissionDefinitionProvider)
        {
            var configuration = DatabaseContextFactory.BuildConfiguration();

            var builder = new DbContextOptionsBuilder<DatabaseContext>()
#if (mysql)
                .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);
#elif (sqlite)     
                .UseSqlite(configuration.GetConnectionString("Default"));
#else
                .UseSqlServer(configuration.GetConnectionString("Default"));
#endif

            using (var dbContext = new DatabaseContext(builder.Options))
            {
                dbContext.Database.Migrate();

                var permissions = permissionDefinitionProvider.GetAllDefinitions();
                foreach (var permission in permissions)
                {
                    var isExsits = await dbContext.Permissions.AnyAsync(x => x.Name == permission);
                    if (!isExsits)
                        dbContext.Permissions.Add(new Permission(permission));
                }
                await dbContext.SaveChangesAsync();

            }
        }
    }
}
