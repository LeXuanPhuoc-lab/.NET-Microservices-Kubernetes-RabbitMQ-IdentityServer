using System.Text.Json;
using AutoMapper;
using Constracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming update auction: " + context.Message.Id);
            Console.WriteLine(JsonSerializer.Serialize(context.Message));

            var auction = context.Message;
            

            if (auction is not null)
            {
                await DB.Update<Auction>()
                .Match(x => x.ID.Equals(auction.Id))
                .Modify(x => x.Item, new Item
                {
                    Make = auction.Make,
                    Model = auction.Model,
                    Year = auction.Year,
                    Color = auction.Color,
                    Mileage = auction.Mileage
                })
                .Modify(x => x.ReservePrice, auction.ReversePrice)
                .ExecuteAsync();
            }
        }
    }
}