using FluentAssertions;
using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Tests.Unit.Models;

public class BasketItemTests
{
    [Fact]
    public void GetTotalPrice_WithNoDiscount_ReturnsCorrectTotal()
    {
        var item = new BasketItem
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 3,
            IsDiscounted = false
        };

        var total = item.GetTotalPrice();

        total.Should().Be(30.00m);
    }

    [Fact]
    public void GetTotalPrice_WithDiscount_ReturnsDiscountedTotal()
    {
        var item = new BasketItem
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Product",
            Price = 100.00m,
            Quantity = 2,
            IsDiscounted = true,
            DiscountPercentage = 20
        };

        var total = item.GetTotalPrice();

        total.Should().Be(160.00m);
    }

    [Fact]
    public void GetTotalPrice_RoundsToTwoDecimalPlaces()
    {
        var item = new BasketItem
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Product",
            Price = 10.333m,
            Quantity = 3,
            IsDiscounted = false
        };

        var total = item.GetTotalPrice();

        total.Should().Be(31.00m);
    }
}

