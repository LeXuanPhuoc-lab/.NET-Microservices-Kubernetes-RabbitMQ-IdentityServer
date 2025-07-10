using System.Globalization;
using System.Text.Json;
using AuctionService.Data;
using AuctionService.DTOS;
using AuctionService.Entities;
using AuctionService.Payloads;
using AuctionService.Payloads.Responses;
using AuctionService.Repos;
using AuctionService.Validations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Constracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndPoint;
        private readonly IElasticGenericRepo<Auction> _repo;

        public AuctionController(AuctionDbContext context, IMapper mapper,
            IPublishEndpoint publishEndpoint,
            IElasticGenericRepo<Auction> repo)
        {
            _context = context;
            _mapper = mapper;
            _publishEndPoint = publishEndpoint;
            _repo = repo;
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
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auctionDtos })
                : NotFound(new BaseResponse() { StatusCode = StatusCodes.Status404NotFound, Message = "Not found any auction" });
        }

        [HttpGet(APIRoutes.Auction.GetAllByDate)]
        public async Task<IActionResult> GetAllAuctionByDate([FromRoute] string date)
        {
            // Get all auction
            var auctions = _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).AsQueryable();

            if (!String.IsNullOrEmpty(date))
            {
                var dateFormat = DateTime.Parse(date).ToUniversalTime();
                auctions = auctions.Where(x => x.UpdatedAt.CompareTo(dateFormat) > 0);
            }

            var auctionDtos = await auctions.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();

            // Response
            return auctionDtos.Count > 0
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auctionDtos })
                : NotFound(new BaseResponse() { StatusCode = StatusCodes.Status404NotFound, Message = "Not found any auction" });
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
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auctionDto })
                : NotFound(new BaseResponse() { StatusCode = StatusCodes.Status404NotFound, Message = $"Not found auction with id {id}" });
        }

        [Authorize]
        [HttpPost(APIRoutes.Auction.Create)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionRequest request)
        {
            // Validate
            var validator = new CreateAuctionValidation();
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = result.ToValidationProblemDetails().Errors
                });
            }
            // Mapping to auction entity 
            var auction = request.ToAuctionEntity();
            auction.Seller = User.Identity?.Name!;
            // Auction id
            var auctionId = Guid.NewGuid();
            Console.WriteLine(auctionId);
            auction.Id = auctionId;

            var newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndPoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            // Add new auction
            await _context.AddAsync(auction);
            await _context.SaveChangesAsync();

            // auction.Item = null;
            // Response
            return CreatedAtRoute(nameof(GetAuctionById), new { Id = auctionId }, newAuction);
        }

        [Authorize]
        [HttpDelete(APIRoutes.Auction.Delete)]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id)
        {
            // check exist
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction is null) return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Not found any auction match id {id}"
            });

            if(auction.Seller != User.Identity?.Name) return Forbid();

            var auctionDto = _mapper.Map<AuctionDto>(auction);

            // Publish RabbitMQ message
            await _publishEndPoint.Publish(_mapper.Map<AuctionDelete>(auctionDto));

            // delete auction
            _context.Auctions.Remove(auction);
            return await _context.SaveChangesAsync() > 0
                ? Ok(new BaseResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"Remove auction {id} successfully"
                })
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPut(APIRoutes.Auction.Update)]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionRequest request)
        {
            // Validation 

            // Convert to auction entity
            var auctionEntity = request.ToAuctionEntity();

            Console.WriteLine("NAME: " + User.Identity?.Name);

            // Get auction by id
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (auction is null) return NotFound(new BaseResponse()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Not found any auction has id {id}"
            });
            if (auction.Seller != User.Identity?.Name) return Forbid();

            // Update fields
            auction.Item.Make = auctionEntity.Item.Make;
            auction.Item.Model = auctionEntity.Item.Model;
            auction.Item.Color = auctionEntity.Item.Color;
            auction.Item.Year = auctionEntity.Item.Year;
            auction.Item.Mileage = auctionEntity.Item.Mileage;
            auction.ReservePrice = auctionEntity.ReservePrice;

            // Map to auction dto
            var auctionDto = _mapper.Map<AuctionDto>(auction);

            // Publish message
            await _publishEndPoint.Publish(new AuctionUpdated()
            {
                Id = auctionDto.Id.ToString(),
                Make = auctionDto.Item.Make,
                Model = auctionDto.Item.Model,
                Year = auctionDto.Item.Year,
                Color = auctionDto.Item.Color,
                Mileage = auctionDto.Item.Mileage,
                ReversePrice = auctionDto.ReservePrice
            });

            // Save change
            var rowEffected = await _context.SaveChangesAsync();
            if (rowEffected > 0)
            {
                return Ok(new BaseResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Update auction succesfully"
                });
            }

            return new ObjectResult(new BaseResponse()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Something went wrong, update auction fail."
            })
            { StatusCode = StatusCodes.Status500InternalServerError };
        }
    
        [HttpPost(APIRoutes.Auction.CreateWithEls)]
        public async Task<IEnumerable<string>> CreateIndex(IEnumerable<CreateAuctionRequest> auctions)
        {
            var auctionDtos = auctions.Select(x => x.ToAuctionEntity());
            return await _repo.Index(_mapper.Map<List<Auction>>(auctionDtos));
        }    

        [HttpGet(APIRoutes.Auction.GetAllWithEls)]
        public async Task<List<Auction>> GetAllWithEls()
            => (await _repo.GetAll()).ToList();

        
        [HttpGet(APIRoutes.Auction.GetByKeyEls)]
        public async Task<Auction> GetByKeyEls(string key)
            => await _repo.Get(key);

        [HttpPost(APIRoutes.Auction.UpdateWithEls)]
        public async Task<bool> UpdateAuctionWithEls([FromRoute] string key, [FromBody] CreateAuctionRequest request)
        {
            var auction = _mapper.Map<Auction>(request);
            return await _repo.Update(auction, key);
        }

        [HttpDelete(APIRoutes.Auction.DeleteWithEls)]
        public async Task<bool> DeleteWithEls(string key)
            => await _repo.Delete(key);
        
    }
}