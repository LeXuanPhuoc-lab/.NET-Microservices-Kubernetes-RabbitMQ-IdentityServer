using AutoMapper;
using BindingService.DTOs;
using BindingService.Models;
using BindingService.Services;
using Constrasts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BindingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController(
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    GrpcAuctionClient grpcClient) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBidAsync(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);
        if (auction == null)
        {
            auction = await grpcClient.GetAuction(auctionId);

            if(auction == null)
            {
                return BadRequest("Cannot access bids on this auction at this time");
            }
        }

        if (auction.Seller == User.Identity.Name)
        {
            return BadRequest("Your cannot bid on your own auction");
        }

        var bid = new Bid()
        {
            AuctionId = auctionId,
            Bidder = User.Identity.Name,
            Amount = amount
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.Status = BidStatus.Finished;
        }
        else
        {
            var highBid = await DB.Find<Bid>()
                .Match(a => a.AuctionId == auctionId)
                .Sort(b => b.Descending(b => b.Amount))
                .ExecuteFirstAsync();

            if (highBid != null && amount > highBid.Amount || highBid == null)
            {
                bid.Status = amount > auction.ReversePrice
                    ? BidStatus.Accepted
                    : BidStatus.AcceptedBelowReserve;
            }

            if (highBid != null && bid.Amount <= highBid.Amount)
            {
                bid.Status = BidStatus.TooLow;
            }
        }

        // Save DB
        await DB.SaveAsync(bid);

        // Process publish event
        await publishEndpoint.Publish(mapper.Map<BidPlaced>(bid));
            
        // Response
        return Ok(mapper.Map<BidDto>(bid));
    }

    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDto>>> GetBidsForAuctionAsync([FromRoute] string auctionId)
    {
        var bids = await DB.Find<Bid>()
            .Match(b => b.AuctionId == auctionId)
            .Sort(b => b.Descending(b => b.BidTime))
            .ExecuteAsync();

        return Ok(bids.Select(mapper.Map<BidDto>).ToList());
    }
}