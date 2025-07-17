using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOS;
using AuctionService.Payloads.Responses;
using AuctionService.Profiles;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests
{
    public class AuctionControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IAuctionRepository> _auctionRepo;
        private readonly Mock<IPublishEndpoint> _pushlishEndpoint;
        private readonly Mapper _mapper;
        private readonly AuctionController _controller;

        public AuctionControllerTests()
        {
            _fixture = new Fixture();
            _auctionRepo = new Mock<IAuctionRepository>();
            _pushlishEndpoint = new Mock<IPublishEndpoint>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(MappingProfile).Assembly);
            }).CreateMapper().ConfigurationProvider;

            _mapper = new Mapper(mockMapper);
            _controller = new AuctionController(_auctionRepo.Object, _mapper, _pushlishEndpoint.Object);
        }

        [Fact]
        public async Task GetAuctions_WithNoParams_Return10Auctions()
        {
            // Arrange
            var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
            _auctionRepo.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

            // Act
            var result = await _controller.GetAllAuctionByDate(null);

            // Assert
            // Not found
            Assert.IsNotType<NotFoundObjectResult>(result);
            // Ok
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(okResult.Value is BaseResponse rs && rs.Data is List<AuctionDto> res && res.Count == 10);
        }

        [Fact]
        public async Task GetAuctionById_WithValidGuid_ReturnAuction()
        {
            // Arrange
            var auction = _fixture.Create<AuctionDto>();
            _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

            // Act
            var result = await _controller.GetAuctionById(auction.Id);

            // Assert
            // Not found
            Assert.IsNotType<NotFoundObjectResult>(result);
            // Ok
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(okResult.Value is BaseResponse rs && rs.Data is AuctionDto res && res != null);
        }

        [Fact]
        public async Task GetAuctionId_WithInvalidGuid_ReturnNotFound()
        {
             // Arrange
            _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

            // Act
            var result = await _controller.GetAuctionById(Guid.NewGuid());

            // Assert
            Assert.IsNotType<OkObjectResult>(result);
            // Not found
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}