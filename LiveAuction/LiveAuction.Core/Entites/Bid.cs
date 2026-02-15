
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites
{
    public class Bid
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; } = default!;
        public Guid BidderId { get; set; }
        public ApplicationUser Bidder { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime PlacedAt { get; set; }
    }
}
