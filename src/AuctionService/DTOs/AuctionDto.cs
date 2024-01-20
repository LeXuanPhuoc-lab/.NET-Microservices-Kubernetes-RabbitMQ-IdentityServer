using AuctionService.Entities;

namespace AuctionService.Data
{
    public class AuctionDto 
    {
        public Guid Id { get; set; }
        public int ReservePrice { get; set; } = 0;  
        public string Seller { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;
        public int? SoldAmount { get; set; } = 0;
        public int? CurrentHighBid { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime AuctionEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public ItemDto Item { get; set; } = null!;
    }
}