using System;
using System.ComponentModel.DataAnnotations;

namespace AuctionService.Payloads
{
    public class UpdateAuctionRequest
    {
        [Required]
        public string Make { get; set; } = string.Empty;
        
        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public string Color { get; set; } = string.Empty;

        [Required]
        public int Mileage { get; set; }

        [Required]
        public int ReversePrice { get; set; }
        
        [Required]
        public DateTime AuctionEnd { get; set; }
    }
}