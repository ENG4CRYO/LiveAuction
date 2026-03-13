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

        public async Task<ProfileRequestDto?> GetUserProfileAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            var profileStatus = await _context.Auctions.AsNoTracking()
                .Where(a => a.Id == id)
                .GroupBy(a => a.Id)
                .Select(g => new
                { 
                    TotalItems = g.Count(),
                    SoldItems = g.Count(a => a.Status == EnAuctionStatus.Sold),
                    AveragePrice = g.Where(a => a.Status == EnAuctionStatus.Sold)
                        .Select(a => (decimal?)a.CurrentPrice)
                        .Average() ?? 0
                }).FirstOrDefaultAsync(cancellationToken);

            var result = new ProfileRequestDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl!,
                Bio = user.Bio!,
                SoldItems = profileStatus?.SoldItems ?? 0,
                TotalItems = profileStatus?.TotalItems ?? 0,
                AvgPrice = profileStatus?.AveragePrice ?? 0
            };
            return result;

        }
    }
}
