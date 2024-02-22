using Constracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionDeletedFaultConsumer : IConsumer<Fault<AuctionDelete>>
    {
        public Task Consume(ConsumeContext<Fault<AuctionDelete>> context)
        {
            Console.WriteLine(context.Message.Exceptions.First().Message);
            
            // using (var session = NHibernateHelper.OpenSession())
            // using (var transaction = session.BeginTransaction())
            // {
            //     var auction = session.Get<AuctionDelete>(context.Message.Message);

            //     // continue with order processing
            //     await context.Publish(auction);
            //     transaction.Commit();
            // }

            throw new Exception("Something went wrong");
        }
    }
}