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
        private readonly IAuctionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndPoint;

        public AuctionController(IAuctionRepository repo, IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _mapper = mapper;
            _publishEndPoint = publishEndpoint;
        }   

        [HttpGet(APIRoutes.Auction.GetAll)]
        public async Task<IActionResult> GetAllAuction()
        {
            // Get all auction
            var auctions = await _repo.GetAuctionsAsync(null);

            // Response
            return auctions.Count > 0
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auctions.ToList() })
                : NotFound(new BaseResponse() { StatusCode = StatusCodes.Status404NotFound, Message = "Not found any auction" });
        }

        [HttpGet(APIRoutes.Auction.GetAllByDate)]
        public async Task<IActionResult> GetAllAuctionByDate([FromRoute] string date)
        {
            // Get all auction
            var auctions = await _repo.GetAuctionsAsync(date);

            // Response
            return auctions.Count > 0
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auctions })
                : NotFound(new BaseResponse() { StatusCode = StatusCodes.Status404NotFound, Message = "Not found any auction" });
        }

        [HttpGet(APIRoutes.Auction.GetById, Name = nameof(GetAuctionById))]
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            // Get auction by id 
            var auction = await _repo.GetAuctionByIdAsync(id);

            // Response
            return auction != null
                ? Ok(new BaseResponse() { StatusCode = StatusCodes.Status200OK, Data = auction })
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
            _repo.AddAuction(auction);
            // Save changes
            var res = await _repo.SaveChangesAsync();
            if (!res) return BadRequest("Failed to save auction");

            // auction.Item = null;
            // Response
            return CreatedAtRoute(nameof(GetAuctionById), new { Id = auctionId }, newAuction);
        }

        [Authorize]
        [HttpDelete(APIRoutes.Auction.Delete)]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id)
        {
            // check exist
            var auction = await _repo.GetAuctionEntityByIdAsync(id);
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
            _repo.RemoveAuction(auction);
            return await _repo.SaveChangesAsync()
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
            var auction = await _repo.GetAuctionEntityByIdAsync(id);
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
            var isSaved = await _repo.SaveChangesAsync();
            if (isSaved)
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
    
        // [HttpPost(APIRoutes.Auction.CreateWithEls)]
        // public async Task<IEnumerable<string>> CreateIndex(IEnumerable<CreateAuctionRequest> auctions)
        // {
        //     var auctionDtos = auctions.Select(x => x.ToAuctionEntity());
        //     return await _repo.Index(_mapper.Map<List<Auction>>(auctionDtos));
        // }    

        // [HttpGet(APIRoutes.Auction.GetAllWithEls)]
        // public async Task<List<Auction>> GetAllWithEls()
        //     => (await _repo.GetAll()).ToList();

        
        // [HttpGet(APIRoutes.Auction.GetByKeyEls)]
        // public async Task<Auction> GetByKeyEls(string key)
        //     => await _repo.Get(key);

        // [HttpPost(APIRoutes.Auction.UpdateWithEls)]
        // public async Task<bool> UpdateAuctionWithEls([FromRoute] string key, [FromBody] CreateAuctionRequest request)
        // {
        //     var auction = _mapper.Map<Auction>(request);
        //     return await _repo.Update(auction, key);
        // }

        // [HttpDelete(APIRoutes.Auction.DeleteWithEls)]
        // public async Task<bool> DeleteWithEls(string key)
        //     => await _repo.Delete(key);
        
    }
}