using System.Net;
using System.Text;
using System.Text.Json;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Payloads.Responses;
using ZstdSharp.Unsafe;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }


        // Only get data had date > lastest date in MongoDB -> Just get lastest data
        public async Task<List<Auction>?> GetItemsForSearchAsync()
        {
            // Get lastest update date
            var lastUpdated = await DB.Find<Auction, string>()
                             .Sort(x => x.Descending(a => a.UpdatedAt))
                             .Project(x => x.UpdatedAt.ToString())
                             .ExecuteFirstAsync();
            

            // Init Http response
            HttpResponseMessage response = null!;

            // Get all data existing in MongoDB
            var listData = await DB.Find<Auction>().ExecuteAsync();
            if(!listData.Any()) // Not found any 
            {
                response = await _httpClient.GetAsync(
                    // Uri end points 
                    _config["AuctionServiceUrl"] + "/api/auctions"
                );
            }else{ // Existing data
                response = await _httpClient.GetAsync(
                    // Uri end points
                    _config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated
                );
            }

            // Mapping Result to Object 
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
            if(result?.Data is not null)
            {
                // Json Options 
                var options = new JsonSerializerOptions(){PropertyNameCaseInsensitive = true};
                // Convert Data response to string
                var itemJson = JsonSerializer.Serialize(result.Data);
                // Convert data response string to List<Auction> object
                var items = JsonSerializer.Deserialize<List<Auction>>(itemJson, options);
                // Exist data -> Add new 
                return items;
            }

            return null!;
        }
    }
}