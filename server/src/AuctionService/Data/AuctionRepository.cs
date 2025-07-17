using AuctionService.DTOS;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionRepository(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddAuction(Auction auction)
        {
            _context.Auctions.Add(auction);
        }

        public async Task<AuctionDto> GetAuctionByIdAsync(Guid id)
        {
            return await _context.Auctions
                .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<Auction> GetAuctionEntityByIdAsync(Guid id)
        {
            return await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<List<AuctionDto>> GetAuctionsAsync(string date)
        {
            // Get all auction
            var auctions = _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).AsQueryable();

            if (!String.IsNullOrEmpty(date))
            {
                var dateFormat = DateTime.Parse(date).ToUniversalTime();
                auctions = auctions.Where(x => x.UpdatedAt.CompareTo(dateFormat) > 0);
            }

            return await auctions.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveAuction(Auction auction)
        {
            _context.Auctions.Remove(auction);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}