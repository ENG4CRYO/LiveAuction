using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiveAuction.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) :
        IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Bid> Bids { get; set; }


    }
}
