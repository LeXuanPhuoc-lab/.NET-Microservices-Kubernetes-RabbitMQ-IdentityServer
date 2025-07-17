using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTest
{
    // Format of test method naming: Method_Senario_ExpectedResult

    [Fact]
    public void HasReservePrice_ReservePriceGreaterThanZero_True()
    {
        // Arrange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 100
        };

        // Act
        var result = auction.HasReservePrice();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasReservePrice_ReservePriceIsZero_True()
    {
        // Arrange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 0
        };

        // Act
        var result = auction.HasReservePrice();

        // Assert
        Assert.False(result);
    }
}
