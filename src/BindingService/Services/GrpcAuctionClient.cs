using AuctionService;
using BindingService.Models;
using Grpc.Core;
using Grpc.Net.Client;

namespace BindingService.Services
{
    public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
    {
        public async Task<Auction> GetAuction(string id)
        {
            logger.LogInformation("Calling GRPC Service with Url {url}", config["GrpcAuction"]);

            var channel = GrpcChannel.ForAddress(
                config["GrpcAuction"],
                new GrpcChannelOptions {
                    Credentials = ChannelCredentials.Insecure
                    });

            var client = new GrpcAuction.GrpcAuctionClient(channel);
            var request = new GetAuctionRequest { Id = id };

            try
            {
                var call = client.GetAuctionAsync(request);
                var reply = await call.ResponseAsync;

                var auction = new Auction
                {
                    ID = reply.Auction.Id,
                    Seller = reply.Auction.Seller,
                    ReversePrice = reply.Auction.ReservePrice,
                    AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd)
                };

                return auction;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not call GRPC Server");
                return null;
            }
        }
    }
}