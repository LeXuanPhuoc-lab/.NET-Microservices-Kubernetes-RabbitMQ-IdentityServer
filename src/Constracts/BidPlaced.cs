namespace Constrasts;

public class BidPlaced
{
    public string Id { get; set; } = string.Empty;
    public string AuctionId { get; set; } = string.Empty;
    public string Bidder { get; set; } = string.Empty;
    public DateTime BidTime { get; set; }
    public int Amount { get; set; } 
    public string BidStatus { get; set; } = string.Empty;
}