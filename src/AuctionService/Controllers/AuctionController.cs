using AuctionService.Data;
using AuctionService.Payloads;
using AuctionService.Payloads.Responses;
using AuctionService.Validations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private AuctionDbContext _context;
        private IMapper _mapper;

        public AuctionController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet(APIRoutes.Auction.GetAll)]
        public async Task<IActionResult> GetAllAuction()
        {
            // Get all auction
            var auctions = await _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
            // Mapping to Dtos
            var auctionDtos = _mapper.Map<IEnumerable<AuctionDto>>(auctions);
            
            // Response
            return auctions.Count > 0 
                ? Ok(new BaseResponse(){StatusCode = StatusCodes.Status200OK, Data = auctionDtos})
                : NotFound(new BaseResponse(){StatusCode = StatusCodes.Status404NotFound, Message = "Not found any auction"});
        }

        [HttpGet(APIRoutes.Auction.GetById, Name = nameof(GetAuctionById))]
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            // Get auction by id 
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
            // Mapping to Dtos 
            var auctionDto = _mapper.Map<AuctionDto>(auction);

            // Response
            return auction != null 
                ? Ok(new BaseResponse(){StatusCode = StatusCodes.Status200OK, Data = auctionDto})
                : NotFound(new BaseResponse(){StatusCode = StatusCodes.Status404NotFound, Message = $"Not found auction with id {id}"});
        }

        [HttpPost(APIRoutes.Auction.Create)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionRequest request)
        {
            // Validate
            var validator = new CreateAuctionValidation();
            var result = await validator.ValidateAsync(request);
            if(!result.IsValid){
                return BadRequest(new ErrorResponse(){
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = result.ToValidationProblemDetails().Errors
                });
            }
            // Mapping to auction entity 
            var auction = request.ToAuctionEntity();
            // Auction id
            var auctionId = Guid.NewGuid();
            auction.Id = auctionId;

            // Add new auction
            await _context.AddAsync(auction);
            await _context.SaveChangesAsync();

            // auction.Item = null;
            // Response
            return CreatedAtRoute(nameof(GetAuctionById), new{Id = auctionId}, _mapper.Map<AuctionDto>(auction));
        }

        [HttpDelete(APIRoutes.Auction.Delete)]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id)
        {
            // check exist
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if(auction is null) return BadRequest(new BaseResponse{
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Not found any auction match id {id}"
            });

            // delete auction
            _context.Auctions.Remove(auction);
            return await _context.SaveChangesAsync() > 0
                ? Ok(new BaseResponse(){StatusCode = StatusCodes.Status200OK, 
                    Message = $"Remove auction {id} successfully"})               
                : StatusCode(StatusCodes.Status500InternalServerError);
        }   
    }
}