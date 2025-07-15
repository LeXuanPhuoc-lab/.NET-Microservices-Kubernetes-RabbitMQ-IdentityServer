using AuctionService.DTOS;
using AuctionService.Entities;
using AutoMapper;
using Constracts;

namespace AuctionService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Auction, AuctionDto>().ReverseMap();
            CreateMap<AuctionDto, AuctionCreated>().ReverseMap();
            CreateMap<AuctionDto, AuctionDelete>().ReverseMap();
            CreateMap<AuctionDto, AuctionUpdated>().ReverseMap();
            CreateMap<ItemDto, ItemCreated>().ReverseMap();
            CreateMap<Item, ItemDto>().ReverseMap();
        }
    }
}