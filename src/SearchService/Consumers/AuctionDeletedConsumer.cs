using System.Text.Json;
using AutoMapper;
using Constracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDelete>
    {
        private readonly IMapper _mapper;

        public AuctionDeletedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionDelete> context)
        {
            Console.WriteLine($"--> Removing auction: {context.Message.Id}");

            var auction = _mapper.Map<Auction>(context.Message);

            Console.WriteLine(JsonSerializer.Serialize(auction));
            await DB.DeleteAsync<Auction>(x => x.ID.Equals(auction.ID));
        }
    }
}