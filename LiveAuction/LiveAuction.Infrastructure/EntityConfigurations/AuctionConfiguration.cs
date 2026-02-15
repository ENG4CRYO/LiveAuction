using LiveAuction.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Infrastructure.EntityConfigurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(1200);

            builder.Property(a => a.StartingPrice)
                .HasPrecision(18,2);

            builder.Property(a => a.CurrentPrice)
                .HasPrecision(18, 2);

            builder.Property(a => a.RowVersion)
                 .IsRowVersion()
                 .IsConcurrencyToken();

            builder.HasOne(a => a.Seller)
                .WithMany(u => u.Auctions)
                .HasForeignKey(a => a.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Category)
                .WithMany(c => c.Auctions)
                .HasForeignKey(a => a.CategoryId);

            builder.HasMany(a => a.Bids)
                .WithOne(b => b.Auction)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
