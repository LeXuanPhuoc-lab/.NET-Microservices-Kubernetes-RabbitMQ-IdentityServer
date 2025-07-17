using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuctionService.IntergrationTests.Util
{
    public static class AuthHelper
    {
        public static Dictionary<string, object> GetBearerForUser(string username)
        {
            return new() { { ClaimTypes.Name, username } };
        }
    }
}