using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constrasts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly ILogger<BidPlaceConsumer> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AuctionFinishedConsumer(
            ILogger<BidPlaceConsumer> logger,
            IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public Task Consume(ConsumeContext<AuctionFinished> context)
        {
            _logger.LogInformation("--> auction finished message received");

            // Notify all clients about the auction finished event
            return _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
        }
    }
}