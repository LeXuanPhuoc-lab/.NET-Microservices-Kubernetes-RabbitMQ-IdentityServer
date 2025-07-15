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
            public const string GetAllByDate = Base + "/auctions/{date}";
            public const string GetById = Base + "/auctions/{id:Guid}";
            public const string Delete = Base + "/auctions/{id:Guid}";
            public const string Update = Base + "/auctions/{id:Guid}";

            // Elastic Search

            public const string CreateWithEls = Base + "/els/index";
            public const string UpdateWithEls = Base + "/els/auctions/{key}";
            public const string DeleteWithEls = Base + "/els/auctions/{key}";
            public const string GetAllWithEls = Base + "/els/auctions";
            public const string GetByKeyEls = Base + "/els/auctions/{key}";
        }
    }
}