using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BindingService.Models;
using Constrasts;
using MassTransit;
using MongoDB.Entities;

namespace BindingService.Services
{
    public class CheckAuctionFinished : BackgroundService
    {
        private readonly ILogger<CheckAuctionFinished> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CheckAuctionFinished(
            ILogger<CheckAuctionFinished> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogInformation("==> Auctions check is stopping"));

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAuctions(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckAuctions(CancellationToken stoppingToken)
        {
            var finishedAuctions = await DB.Find<Auction>()
                .Match(a => a.AuctionEnd <= DateTime.UtcNow)
                .Match(a => !a.Finished)
                .ExecuteAsync(stoppingToken);

            if (finishedAuctions.Count == 0) return;

            _logger.LogInformation("==> Found {count} auctions completed", finishedAuctions.Count);

            using var scope = _serviceProvider.CreateScope();
            var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach (var auction in finishedAuctions)
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                var winningBid = await DB.Find<Bid>()
                    .Match(b => b.AuctionId == auction.ID)
                    .Match(b => b.Status == BidStatus.Accepted)
                    .Sort(b => b.Descending(b => b.Amount))
                    .ExecuteFirstAsync(stoppingToken);

                await endpoint.Publish(new AuctionFinished()
                {
                    ItemSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount ?? 0,
                    Seller = auction.Seller,
                }, stoppingToken);
            }
        }
    }
}