using Constrasts;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await DB.Find<Auction>().OneAsync(context.Message.AuctionId);

        if (context.Message.BidStatus.Contains("Accepted")
         && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}