using System.Data;
using AuctionService.Entities;
using AuctionService.Payloads;
using FluentValidation;

namespace AuctionService.Validations
{
    public class CreateAuctionValidation : AbstractValidator<CreateAuctionRequest>
    {
        public CreateAuctionValidation()
        {
            // Adding validation features here 
            RuleFor(x => x.ImageUrl)
                .Matches(@"asdasd");
        }
    }
}