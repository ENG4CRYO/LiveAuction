using LiveAuction.Application.Interfaces.RepositoryInterfaces;
using LiveAuction.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
