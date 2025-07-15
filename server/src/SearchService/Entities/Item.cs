using MongoDB.Entities;

namespace SearchService.Entities
{
    public class Item : Entity
    {
        // Mongo DB Entity already has its own ID property
        // public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Mileage { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        //public string AuctionId {get;set;} = string.Empty;
    }
}