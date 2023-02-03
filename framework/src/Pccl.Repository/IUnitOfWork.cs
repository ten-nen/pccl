﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pccl.Repository
{
    public interface IUnitOfWork: IDisposable
    {
        void BeginTransaction();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        void CommitTransaction();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        void RollbackTransaction();
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
