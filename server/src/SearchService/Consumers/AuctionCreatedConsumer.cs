using System.Text.Json;
using AutoMapper;
using Constracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        public readonly IMapper _mapper;
        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming Auction Created: " + context.Message.Id);

            var item = _mapper.Map<Auction>(context.Message);
            await DB.SaveAsync(item);
        }
    }
}