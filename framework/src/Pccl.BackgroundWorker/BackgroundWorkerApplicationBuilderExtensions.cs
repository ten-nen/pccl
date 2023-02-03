using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Pccl.BackgroundWorker
{
    public static class BackgroundWorkerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBackgroundWorker(this IApplicationBuilder builder, Action<JobExceptionInfo> onJobException = null)
        {
            var appLitetime = builder.ApplicationServices.GetService<IHostApplicationLifetime>();
            appLitetime.ApplicationStarted.Register(() =>
            {
                JobManager.Initialize();
                var works = builder.ApplicationServices.GetServices<IWork>();
                foreach (var work in works)
                {
                    JobManager.AddJob(work, schedule => work.Configure(schedule));
                }
                if (onJobException != null)
                    JobManager.JobException += info => onJobException.Invoke(info);
            });
            appLitetime.ApplicationStopping.Register(() => JobManager.Stop());
            return builder;
        }
    }
}
