using AuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions options):base(options)
        {
        }

        // Tell the EntityFramwork for Entities
        public DbSet<Auction> Auctions { get; set; }

        //DbSet<Item> Items { get; set; }
        //-> That's not necessary because of inside Items.cs was related to Auction.cs

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add new 3 table into Database by Migration
            // In charge of outbox functionality
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
