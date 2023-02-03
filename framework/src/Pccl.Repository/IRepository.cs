using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pccl.Repository
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        T Add(T entity);

        Task<T> AddAsync(T entity);

        void AddRange(IEnumerable<T> entities);

        T Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        T Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }

}