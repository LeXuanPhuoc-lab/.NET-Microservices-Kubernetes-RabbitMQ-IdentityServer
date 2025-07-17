

using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.IntergrationTests.Util;
using AuctionService.Payloads;
using Constracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntergrationTests
{
    [Collection("Shared collection")]
    public class AuctionBusTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private HttpClient _httpClient;
        private ITestHarness _testHarness;

        public AuctionBusTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _testHarness = _factory.Services.GetTestHarness();
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(await _testHarness.Published.Any<AuctionCreated>());
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