using FluentAssertions;
using FreemarketFxAbdullahTask.Services;

namespace FreemarketFxAbdullahTask.Tests.Unit.Services;

public class ValidationServiceTests
{
    private readonly ValidationService _service;

    public ValidationServiceTests()
    {
        _service = new ValidationService();
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100)]
    public void ValidatePrice_ReturnsTrue_ForPositivePrices(decimal price)
    {
        var result = _service.ValidatePrice(price);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ValidatePrice_ReturnsFalse_ForNonPositivePrices(decimal price)
    {
        var result = _service.ValidatePrice(price);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void ValidateQuantity_ReturnsTrue_ForPositiveQuantities(int quantity)
    {
        var result = _service.ValidateQuantity(quantity);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidateQuantity_ReturnsFalse_ForNonPositiveQuantities(int quantity)
    {
        var result = _service.ValidateQuantity(quantity);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void ValidateDiscountPercentage_ReturnsTrue_ForValidPercentages(decimal percentage)
    {
        var result = _service.ValidateDiscountPercentage(percentage);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ValidateDiscountPercentage_ReturnsFalse_ForInvalidPercentages(decimal percentage)
    {
        var result = _service.ValidateDiscountPercentage(percentage);

        result.Should().BeFalse();
    }
}

