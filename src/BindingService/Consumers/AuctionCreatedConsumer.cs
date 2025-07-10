using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BindingService.Models;
using Constracts;
using MassTransit;
using MongoDB.Entities;

namespace BindingService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            var auction = new Auction()
            {
                ID = context.Message.Id.ToString(),
                Seller = context.Message.Seller,
                AuctionEnd = context.Message.AuctionEnd,
                ReversePrice = context.Message.ReservePrice
            };

            await auction.SaveAsync();
        }
    }
}