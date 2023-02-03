using FluentScheduler;
using Pccl.BackgroundWorker;
using Pccl.ProjectTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Pccl.ProjectTemplate.Api.Jobs
{
    public class DemoJob : IWork
    {
        private DbContextOptions<DatabaseContext> _dbOptions;
        private IServiceScope _scope;
        public DemoJob(IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
            _dbOptions = _scope.ServiceProvider.GetService<DbContextOptions<DatabaseContext>>();
        }
        public Schedule Configure(Schedule schedule)
        {
            schedule.ToRunEvery(1).Days().At(10, 28);  //每天早上10:14
            return schedule;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("executing.." + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("executing..");
            System.Diagnostics.Debug.WriteLine("executing..");

            //如果不想使用数据库审计
            using (var db = new DatabaseContext(_dbOptions))
            {
                var user = db.Users.FirstOrDefault();
                System.Diagnostics.Debug.WriteLine("executing.. User:" + user?.Name);
            }

            System.Diagnostics.Debug.WriteLine("executed.." + DateTime.Now);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
