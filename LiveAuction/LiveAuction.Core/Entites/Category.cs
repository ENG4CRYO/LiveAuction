using LiveAuction.Core.Entites.BaseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites
{
    public class Category : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Auction> Auctions { get; set; } = default!;
    }
}
