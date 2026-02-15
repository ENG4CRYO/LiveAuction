using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity;

namespace LiveAuction.Core.Entites
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get;set; }
        public ICollection<Auction> Auctions { get; set; } = default!;
        public ICollection<Bid> Bids { get; set; } = default!;
    }
}
