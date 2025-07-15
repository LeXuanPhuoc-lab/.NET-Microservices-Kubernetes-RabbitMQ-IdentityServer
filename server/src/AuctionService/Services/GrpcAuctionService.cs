using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services
{
    public class GrpcAuctionService(AuctionDbContext dbContext) : GrpcAuction.GrpcAuctionBase
    {
        public override async Task<GrpcAuctionResponse> GetAuction(
            GetAuctionRequest req,
            ServerCallContext context)
        {
            Console.WriteLine("==> Recieve Grpc request for auction");

            var auction = await dbContext.Auctions.FindAsync(Guid.Parse(req.Id));
            if (auction == null) throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

            var res = new GrpcAuctionResponse
            {
                Auction = new GrpcAuctionModel()
                {
                    AuctionEnd = auction.AuctionEnd.ToString(),
                    Id = auction.Id.ToString(),
                    ReservePrice = auction.ReservePrice,
                    Seller = auction.Seller,
                }
            };

            return res;
        }
    }
}