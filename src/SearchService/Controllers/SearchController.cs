using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Entities;
using SearchService.Common.Constants;
using SearchService.Entities;
using SearchService.Payloads;
using SearchService.Payloads.Requests;
using SearchService.Payloads.Responses;
using SearchService.Utils;

namespace SearchService.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public SearchController(IOptionsMonitor<AppSettings> monitor)
        {
            _appSettings = monitor.CurrentValue;
        }

        [HttpGet(APIRoutes.Search.Filter)]
        public Task<IActionResult> ShowFilter()
        {
            return Task.FromResult<IActionResult>(Ok(new BaseResponse{
                StatusCode = StatusCodes.Status200OK,
                Data = new {
                    FilterBy = new[] {FilterBy.FINISHED, FilterBy.ENDINGSOON},
                    OrderBy = new[] {OrderBy.NEW, OrderBy.MAKE}
                }
            }));
        }

        [HttpGet(APIRoutes.Search.SearchByValue)]
        public async Task<IActionResult> SearchByValue([FromQuery] SearchRequest request)
        {
            // Create MongoDB Find command 
            var query = DB.Find<Auction,Auction>();
            
            // // Order items
            // query = request.OrderBy?.ToUpper() switch
            // {
            //     OrderBy.MAKE => query.Sort(x => x.Ascending(a => a.Item.Make)),
            //     OrderBy.NEW => query.Sort(x => x.Ascending(a => a.CreatedAt)),
            //     // default
            //     _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            // };

            // Filter Item
            query = request.FilterBy?.ToUpper() switch 
            {
                FilterBy.FINISHED => query.Match(x => x.AuctionEnd > DateTime.UtcNow.Subtract(TimeSpan.FromDays(10))),
                FilterBy.ENDINGSOON => query.Match(x => 
                    x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                 && x.AuctionEnd < DateTime.UtcNow),
                // default
                _ => query.Match(x => x.AuctionEnd < DateTime.UtcNow)
            };

            // // Search by winner 
            // if(!String.IsNullOrEmpty(request.Winner))
            // {
            //     query.Match(x => x.Winner == request.Winner);
            // }
            // // Search by seller
            // if(!String.IsNullOrEmpty(request.Seller))
            // {
            //     query.Match(x => x.Seller == request.Seller);
            // }
            // Search by value 
            if(!String.IsNullOrEmpty(request.SearchValue))
            {
                // Find by value and Sort the results of a text search by the MetaTextScore
                query.Match(Search.Full, request.SearchValue).SortByTextScore(); // Multiple data search
            }
            // Sorting ascending by Make property
            query.Sort(x => x.Ascending(a => a.Item.Make));

            // Execute and return List<Auction> 
            var result = await query.ExecuteAsync();
            
            // Paging
            var pagingItems = PaginatedList<Auction>.Paging(result, request.PageIndex, _appSettings.PageSize);

            return result.Count > 0 
                ? Ok(new BaseResponse{StatusCode = StatusCodes.Status200OK, Data = new{
                    Result = pagingItems.ToList(),
                    PageIndex = pagingItems.PageIndex,
                    TotalPage = pagingItems.TotalPage
                }})
                : NotFound(new BaseResponse{
                    StatusCode = StatusCodes.Status200OK, 
                    Message = $"Not found any match {request.SearchValue}"
                }); 
        }

        // [HttpGet(APIRoutes.Search.SearchAll)]
        // public async Task<IActionResult> SearchByValue(int pageIndex = 1)
        // {
        //     // Create MongoDB Find command 
        //     var query = DB.Find<Auction>();
            
        //     // Sorting ascending by Make property
        //     query.Sort(x => x.Ascending(a => a.Item.Make));

        //     // Execute and return List<Auction> 
        //     var result = await query.ExecuteAsync();

        //     // Paging 
        //     var pagingItems = PaginatedList<Auction>.Paging(result, pageIndex, _appSettings.PageSize);

        //     return result.Count > 0 
        //         ? Ok(new BaseResponse{StatusCode = StatusCodes.Status200OK, Data = new{
        //             Result = pagingItems.ToList(),
        //             PageIndex = pagingItems.PageIndex,
        //             TotalPage = pagingItems.TotalPage
        //         }})
        //         : NotFound(new BaseResponse{
        //             StatusCode = StatusCodes.Status200OK, 
        //             Message = "Not found any auction"
        //         }); 
        // }

    } 
}