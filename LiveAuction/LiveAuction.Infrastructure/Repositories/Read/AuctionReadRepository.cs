using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Application.Interfaces.RepositoryInterfaces.Read;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Enums;
using LiveAuction.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Infrastructure.Repositories.Read
{
    public class AuctionReadRepository : GenericReadRepository<Auction>,IAuctionReadRepository 
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuctionReadRepository(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<(ApplicationUser User, int TotalItems, int SolidItems, decimal? AvgPrice)?>
            GetUserProfileAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _context.Users.AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    User = u,
                    TotalItems = u.Auctions.Count(),
                    SoldItems = u.Auctions.Count(a => a.Status == EnAuctionStatus.Sold),
                    AvgPrice = u.Auctions.Where(a => a.Status == EnAuctionStatus.Sold).Any()    
                        ? u.Auctions.Where(a => a.Status == EnAuctionStatus.Sold)
                        .Average(a => a.CurrentPrice)
                        : 0
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null) return null;

            return (result.User, result.TotalItems, result.SoldItems, result.AvgPrice);

        }
    }
}
