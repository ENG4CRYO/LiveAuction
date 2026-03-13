using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces.RepositoryInterfaces.Read
{
    public interface IAuctionReadRepository : IGenericReadRepository<Auction>
    {
        Task<ProfileRequestDto?> GetUserProfileAsync(Guid id,CancellationToken cancellationToken);
    }
}
