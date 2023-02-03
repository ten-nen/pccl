using FluentScheduler;
using System;

namespace Pccl.BackgroundWorker
{
    public interface IWork : IJob, IDisposable
    {
        Schedule Configure(Schedule schedule);
    }
}
