using FluentAssertions;
using FreemarketFxAbdullahTask.Models;
using FreemarketFxAbdullahTask.Services;

namespace FreemarketFxAbdullahTask.Tests.Unit.Services;

public class DiscountServiceTests
{
    private readonly DiscountService _service;

    public DiscountServiceTests()
    {
        _service = new DiscountService();
    }

    [Fact]
    public void CalculateItemDiscount_ReturnsZero_WhenItemNotDiscounted()
    {
        var item = new BasketItem
        {
            Price = 100.00m,
            Quantity = 1,
            IsDiscounted = false,
            ProductName = "Item"
        };

        var discount = _service.CalculateItemDiscount(item);

        discount.Should().Be(0);
    }

    [Fact]
    public void CalculateItemDiscount_CalculatesCorrectly_WhenItemDiscounted()
    {
        var item = new BasketItem
        {
            Price = 100.00m,
            Quantity = 2,
            IsDiscounted = true,
            DiscountPercentage = 20,
            ProductName = "Item"
        };

        var discount = _service.CalculateItemDiscount(item);

        discount.Should().Be(40.00m);
    }

    [Fact]
    public void CanApplyDiscountCode_ReturnsFalse_ForDiscountedItems()
    {
        var item = new BasketItem
        {
            IsDiscounted = true,
            ProductName = "Item"
        };

        var result = _service.CanApplyDiscountCode(item);

        result.Should().BeFalse();
    }
}

