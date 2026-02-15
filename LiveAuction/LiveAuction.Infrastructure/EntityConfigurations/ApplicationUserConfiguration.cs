using LiveAuction.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Infrastructure.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.Auctions)
                .WithOne(a => a.Seller)
                .HasForeignKey(a => a.SellerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(u => u.Bids)
                .WithOne(b => b.Bidder)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FirstName).HasMaxLength(50);
            builder.Property(u => u.LastName).HasMaxLength(50);
        }
    }
}
