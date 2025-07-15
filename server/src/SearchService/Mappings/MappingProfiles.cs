using AutoMapper;
using Constracts;
using SearchService.Entities;

namespace SearchService.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AuctionCreated, Auction>().ReverseMap();
            CreateMap<AuctionDelete, Auction>().ReverseMap();
            CreateMap<AuctionUpdated, Auction>().ReverseMap();
            CreateMap<ItemCreated, Item>().ReverseMap();
        }
    }
}