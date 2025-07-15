using System;
using System.ComponentModel.DataAnnotations;
using AuctionService.Entities;

namespace AuctionService.Payloads
{
    public class CreateAuctionRequest
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
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int ReversePrice { get; set; }
        
        [Required]
        public DateTime AuctionEnd { get; set; }
    }

    public static class CreateAuctionRequestExtension
    {
        public static Auction ToAuctionEntity(this CreateAuctionRequest request)
        {
            return new Auction()
            {
                Item = new Item()
                {
                    Make = request.Make,
                    Color = request.Color,
                    Model = request.Model,
                    Year = request.Year,
                    Mileage = request.Mileage,
                    ImageUrl = request.ImageUrl
                },
                ReservePrice = request.ReversePrice,
                AuctionEnd = request.AuctionEnd
            };
        }
    }
}