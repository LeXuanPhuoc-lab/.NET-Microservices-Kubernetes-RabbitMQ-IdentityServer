using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BindingService.DTOs;
using BindingService.Models;
using Constrasts;

namespace BindingService.Mapping
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<Bid, BidDto>()
            .ForMember(
                dest => dest.BidStatus,
                opt => opt.MapFrom(src => src.Status.ToString())
            )
            .ReverseMap();


            CreateMap<Bid, BidPlaced>()
            .ForMember(
                dest => dest.BidStatus,
                opt => opt.MapFrom(src => src.Status.ToString())
            )
            .ReverseMap();
        }
    }
}