using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BindingService.DTOs
{
    public class BidDto
    {
        public string Id { get; set; }
        public string AuctionId { get; set; }
        public string Bidder { get; set; }
        public DateTime BidTime { get; set; } = DateTime.UtcNow;
        public int Amount { get; set; }
        public string BidStatus { get; set; }
    }
}