using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pccl.Repository;
using Pccl.UnitTests;
using Pccl.Audit;
using Pccl.Cache;

namespace ProjectTemplate.UnitTests
{
    public class InfrastructureTest : TestBase
    {
        private readonly ServiceProvider serviceProvider;
        public InfrastructureTest()
        {
            var loggerFactory = new LoggerFactory()
                     .AddSerilog(new LoggerConfiguration()
                                     .WriteTo.File("logs/logs-.txt", rollingInterval: RollingInterval.Day)
                                     .CreateLogger());

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                                        {
                                            { "ConnectionStrings:Default", "Password=123456;Persist Security Info=True;User ID=sa;Initial Catalog=ProjectTemplate;Data Source=.;" }
                                        })
                .Build();

            var sc = new ServiceCollection();
            sc.AddCache();
            serviceProvider = sc
            .AddScoped<IConfiguration>(_ => configuration)
            .AddOptions()
            .AddSingleton(loggerFactory)
            .AddAudit()
            .AddDbContext<TestDbContext>((serviceProvider, options) =>
                                options.UseSqlServer(configuration.GetConnectionString("Default"))
                                       .AddAuditInterceptors(serviceProvider))
            .AddRepositories(typeof(TestDbContext).Assembly) //配置仓储
            .BuildServiceProvider();
        }

        [Fact]
        public async void TestAudit()
        {
            var dbContext = serviceProvider.GetRequiredService<TestDbContext>();
            var model = new Setting("dbcontext", "222", "222");
            dbContext.Settings.Add(model);
            dbContext.SaveChanges();
            Assert.True(model.CreatedTime.Year > 2000);

            model.SetDescription("222");
            dbContext.Update(model);
            await dbContext.SaveChangesAsync();
            Assert.NotNull(model.ModifiedTime);
        }

        [Fact]
        public async void TestRepository()
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            var repository = serviceProvider.GetRequiredService<ISettingRepository>();
            var model = new Setting("repository", "111", "111");
            await repository.AddAsync(model);
            var count = await uow.SaveChangesAsync();
            Assert.True(count > 0);
            Assert.NotEqual(model.Id, Guid.Empty);
        }
    }



    public interface ISettingRepository : IRepository<Setting>
    {
    }
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public TestDbContext Context { get; set; }
        public SettingRepository(TestDbContext dbContext, ICacheStoreProvider cacheStoreProvider) : base(dbContext, cacheStoreProvider)
        {
            Context = dbContext;
        }
    }
    public class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot where TKey : IEquatable<TKey>
    {
    }
    public class AuditedAggregateRoot<TKey> : AuditedEntity<TKey>, IAggregateRoot where TKey : IEquatable<TKey>
    {
    }
    public class Setting : AuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Description { get; private set; }

        private Setting()
        {
            // required by EF
        }

        public Setting(string name, string value, string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }

        public void SetName(string name)
        {
            Name = name;
        }
        public void SetValue(string value)
        {
            Value = value;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
    }
    public static class SettingModelBuilderExtensions
    {
        public static void ConfigureSetting(this ModelBuilder modelBuilder)
        {
            const string dbTablePrefix = "ProjectTemplate_";
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
    public class TestDbContext : DbContext, IUnitOfWork
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }
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
            builder.ConfigureSetting();
        }
    }
}
