using AuctionService.Entities;
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
    }
}
