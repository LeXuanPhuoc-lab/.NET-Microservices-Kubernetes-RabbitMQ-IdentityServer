
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AuctionService.Data;
using AuctionService.DTOS;
using AuctionService.IntergrationTests.Util;
using AuctionService.Payloads;
using AuctionService.Payloads.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntergrationTests
{
    [Collection("Shared collection")]
    public class AuctionControllerTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private HttpClient _httpClient;

        private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

        public AuctionControllerTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAuctions_ShouldReturn3Auctions()
        {
            // Arrange?

            // Act
            var response = await _httpClient.GetFromJsonAsync<BaseResponse>("/api/auctions");
            var element = (JsonElement)response.Data!;
            var count = element.GetArrayLength();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
        {
            // Arrange?

            // Act
            var response = await _httpClient.GetFromJsonAsync<BaseResponse>($"/api/auctions/{GT_ID}");
            var element = (JsonElement)response.Data!;

            // Assert
            var auction = element.Deserialize<AuctionDto>();
            Assert.IsType<AuctionDto>(auction);
        }

        [Fact]
        public async Task GetAuctionById_WithInValidId_ShouldReturn404()
        {
            // Arrange?

            // Act
            var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithNoAuth_ShouldReturn401()
        {
            // Arrange
            var auction = new CreateAuctionRequest() { Make = "Test" };

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithAuth_Return201()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            DbHelper.ReInitDbForTests(db);

            return Task.CompletedTask;
        }

        private CreateAuctionRequest GetAuctionForCreate()
        {
            return new()
            {
                Make = "test",
                Model = "testModel",
                ImageUrl = "test",
                Color = "test",
                Mileage = 10,
                Year = 10,
                ReversePrice = 10
            };
        }
    }
}