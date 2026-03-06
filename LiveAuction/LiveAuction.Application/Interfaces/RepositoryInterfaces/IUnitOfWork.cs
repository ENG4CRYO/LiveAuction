using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces.RepositoryInterfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
