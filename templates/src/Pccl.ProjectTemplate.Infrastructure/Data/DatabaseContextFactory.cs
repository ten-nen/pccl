using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pccl.ProjectTemplate.Infrastructure.Data
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<DatabaseContext>()
#if (mysql)
                .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);
#elif (sqlite)     
                .UseSqlite(configuration.GetConnectionString("Default"));
#else
                .UseSqlServer(configuration.GetConnectionString("Default"));
#endif

            return new DatabaseContext(builder.Options);
        }

        public static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Pccl.ProjectTemplate.Api/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
