using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities
{
    [Table("Items")]
    public class Item
    {
        [Key]
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Mileage { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // nav properties
        [ForeignKey(nameof(Auction))]
        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;
    }
}
