using MongoDB.Entities;

namespace BindingService.Models;

public class Auction : Entity
{
    public DateTime AuctionEnd { get; set; }
    public string Seller { get; set; }
    public int ReversePrice { get; set; }
    public bool Finished { get; set; }
}
