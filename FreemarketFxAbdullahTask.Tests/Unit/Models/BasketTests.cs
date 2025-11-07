using FluentAssertions;
using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Tests.Unit.Models;

public class BasketTests
{
    [Fact]
    public void GetSubtotal_WithMultipleItems_ReturnsCorrectSum()
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
            {
                new() { Price = 10.00m, Quantity = 2, IsDiscounted = false, ProductName = "Item 1" },
                new() { Price = 15.00m, Quantity = 1, IsDiscounted = false, ProductName = "Item 2" }
            }
        };

        var subtotal = basket.GetSubtotal();

        subtotal.Should().Be(35.00m);
    }

    [Fact]
    public void GetDiscountAmount_ExcludesDiscountedItems()
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
            {
                new() { Price = 100.00m, Quantity = 1, IsDiscounted = false, ProductName = "Regular Item" },
                new() { Price = 50.00m, Quantity = 1, IsDiscounted = true, DiscountPercentage = 10, ProductName = "Discounted Item" }
            },
            DiscountPercentage = 10
        };

        var discountAmount = basket.GetDiscountAmount();

        discountAmount.Should().Be(10.00m);
    }

    [Fact]
    public void GetTotalWithVat_AppliesTwentyPercentVat()
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
            {
                new() { Price = 100.00m, Quantity = 1, IsDiscounted = false, ProductName = "Item" }
            }
        };

        var totalWithVat = basket.GetTotalWithVat();

        totalWithVat.Should().Be(120.00m);
    }

    [Fact]
    public void GetTotalWithoutVat_SubtractsDiscountFromSubtotal()
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
            {
                new() { Price = 100.00m, Quantity = 1, IsDiscounted = false, ProductName = "Item" }
            },
            DiscountCode = "SAVE20",
            DiscountPercentage = 20
        };

        var totalWithoutVat = basket.GetTotalWithoutVat();

        totalWithoutVat.Should().Be(80.00m);
    }
}

