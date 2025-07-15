using Constrasts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
    public class BidPlaceConsumer : IConsumer<BidPlaced>
    {
        private readonly ILogger<BidPlaceConsumer> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public BidPlaceConsumer(
            ILogger<BidPlaceConsumer> logger,
            IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public Task Consume(ConsumeContext<BidPlaced> context)
        {
            _logger.LogInformation("--> bid placed message received");

            // Notify all clients about the bid placed event
            return _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
        }
    }
}