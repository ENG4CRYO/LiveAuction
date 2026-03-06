using LiveAuction.Core.Entites.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LiveAuction.Application.Interfaces.RepositoryInterfaces.Read
{
    public interface IGenericReadRepository<T> where T : class,IEntity
    {
        Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken);
        Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    }
}
