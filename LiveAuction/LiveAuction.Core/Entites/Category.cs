using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Auction> Auctions { get; set; } = default!;
    }
}
