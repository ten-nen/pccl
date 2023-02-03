using Pccl.Cache;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace Pccl.Repository
{
    public abstract class RepositoryBase<T> : IRepository<T>, IHasCacheStore where T : class, IAggregateRoot
    {
        protected readonly DbContext DbContext;

        public ICacheStoreProvider CacheStoreProvider { get; }
        public RepositoryBase(DbContext dbContext, ICacheStoreProvider cacheStoreProvider)
        {
            DbContext = dbContext;
            CacheStoreProvider = cacheStoreProvider;
        }

        public virtual T Add(T entity) => DbContext.Set<T>().Add(entity).Entity;

        public virtual async Task<T> AddAsync(T entity) => (await DbContext.Set<T>().AddAsync(entity)).Entity;

        public virtual void AddRange(IEnumerable<T> entities) => DbContext.Set<T>().AddRange(entities);

        public virtual T Update(T entity) => DbContext.Set<T>().Update(entity).Entity;

        public virtual void UpdateRange(IEnumerable<T> entities) => DbContext.Set<T>().UpdateRange(entities);

        public virtual T Remove(T entity) => DbContext.Set<T>().Remove(entity).Entity;

        public virtual void RemoveRange(IEnumerable<T> entities) => DbContext.Set<T>().RemoveRange(entities);

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await DbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

    }
}
