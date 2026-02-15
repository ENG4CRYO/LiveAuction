using LiveAuction.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites
{
    public class Auction
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public ApplicationUser Seller { get; set; } = new ApplicationUser();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; } 
        public Category Category { get; set; } = new Category();
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal? CurrentPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public EnAuctionStatus Status { get; set; }
        public uint RowVersion { get; set; }

        public ICollection<Bid> Bids { get; set; } = default!;
    }
}
