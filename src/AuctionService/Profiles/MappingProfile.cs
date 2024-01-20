using AuctionService.Data;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Auction, AuctionDto>().ReverseMap();
            CreateMap<Item, ItemDto>().ReverseMap();
        }
    }
}