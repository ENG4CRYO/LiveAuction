
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites
{
    public class Bid
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; } = new Auction();
        public Guid BidderId { get; set; }
        public ApplicationUser Bidder { get; set; } = new ApplicationUser();
        public decimal Amount { get; set; }
        public DateTime PlacedAt { get; set; }
    }
}
