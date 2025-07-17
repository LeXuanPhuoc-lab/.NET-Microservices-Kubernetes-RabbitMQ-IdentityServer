using AuctionService.DTOS;
using AuctionService.Entities;

namespace AuctionService.Data
{
    public interface IAuctionRepository
    {
        Task<List<AuctionDto>> GetAuctionsAsync(string date);
        Task<AuctionDto> GetAuctionByIdAsync(Guid id);
        Task<Auction> GetAuctionEntityByIdAsync(Guid id);
        void AddAuction(Auction auction);
        void RemoveAuction(Auction auction);
        Task<bool> SaveChangesAsync();
    }
}