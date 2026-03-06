using LiveAuction.Application.Interfaces.RepositoryInterfaces.Read;
using LiveAuction.Core.Entites.BaseEntity;
using LiveAuction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LiveAuction.Infrastructure.Repositories.Read
{
    public class GenericReadRepository<T> : IGenericReadRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _context;
        public GenericReadRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate,cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
    }
}
