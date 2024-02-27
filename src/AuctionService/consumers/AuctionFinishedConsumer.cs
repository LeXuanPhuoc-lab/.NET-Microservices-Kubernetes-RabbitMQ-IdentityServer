using AuctionService.Data;
using AuctionService.Entities;
using Constrasts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbContext;

    public AuctionFinishedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("--> Consuming Auction Finished");

        var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction != null && context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
            auction.Status = auction.SoldAmount > 0 
            ? Status.Finished 
            : Status.ReserveNotMet;
        }

        // Save DB Change
        await _dbContext.SaveChangesAsync();
    }
}