namespace Constrasts;

public class AuctionFinished
{
    public bool ItemSold { get; set; }
    public string AuctionId { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public string Seller { get; set; } = string.Empty;
    public int? Amount { get; set; }
}