using System.Diagnostics.CodeAnalysis;

namespace SearchService.Payloads
{
    public static class APIRoutes
    {
        public const string Base = "api";

        public static class Search
        {
            public const string SearchByValue = Base + "/search";
            public const string SearchAll = Base + "/search/all";
            public const string Filter = Base + "/search/filter";
        }
    }
}