using LanguageExt.TypeClasses;

namespace AuctionService.Payloads
{
    public class APIRoutes
    {
        public const string Base = "api";

        public static class Auction
        {
            public const string Create = Base + "/auctions";
            public const string GetAll = Base + "/auctions";
            public const string GetById = Base + "/auctions/{id:Guid}";
            public const string Delete = Base + "/auctions/{id:Guid}";
        }
    }
}