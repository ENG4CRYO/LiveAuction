using LiveAuction.Core.Entites.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LiveAuction.Application.Interfaces.RepositoryInterfaces.Write
{
    public interface IGenericWriteRepository<T> where T : class, IEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task UpdateRangeAsync(IEnumerable<T> entities);
    }
}
