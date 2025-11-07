using FluentAssertions;
using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using FreemarketFxAbdullahTask.Queries;
using FreemarketFxAbdullahTask.Queries.Handlers;
using FreemarketFxAbdullahTask.Services;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Tests.Unit.Queries;

public class GetBasketTotalQueryHandlerTests
{
    private readonly BasketDbContext _context;
    private readonly GetBasketTotalQueryHandler _handler;

    public GetBasketTotalQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<BasketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new BasketDbContext(options);

        var vatService = new VatCalculationService();
        var calculationService = new BasketCalculationService(vatService);
        _handler = new GetBasketTotalQueryHandler(_context, calculationService);
    }

    [Fact]
    public async Task HandleAsync_CalculatesTotalWithVat_Correctly()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = basketId,
            Items = new List<BasketItem>
            {
                new() { Id = Guid.NewGuid(), BasketId = basketId, ProductName = "Item", Price = 100.00m, Quantity = 1, IsDiscounted = false }
            }
        };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var query = new GetBasketTotalQuery { BasketId = basketId, IncludeVat = true };

        var total = await _handler.HandleAsync(query);

        total.Should().Be(120.00m);
    }

    [Fact]
    public async Task HandleAsync_CalculatesTotalWithoutVat_Correctly()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = basketId,
            Items = new List<BasketItem>
            {
                new() { Id = Guid.NewGuid(), BasketId = basketId, ProductName = "Item", Price = 100.00m, Quantity = 1, IsDiscounted = false }
            }
        };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var query = new GetBasketTotalQuery { BasketId = basketId, IncludeVat = false };

        var total = await _handler.HandleAsync(query);

        total.Should().Be(100.00m);
    }

    [Fact]
    public async Task HandleAsync_AppliesDiscountCode_ToEligibleItems()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = basketId,
            Items = new List<BasketItem>
            {
                new() { Id = Guid.NewGuid(), BasketId = basketId, ProductName = "Regular Item", Price = 100.00m, Quantity = 1, IsDiscounted = false },
                new() { Id = Guid.NewGuid(), BasketId = basketId, ProductName = "Discounted Item", Price = 50.00m, Quantity = 1, IsDiscounted = true, DiscountPercentage = 10 }
            },
            DiscountCode = "SAVE10",
            DiscountPercentage = 10
        };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var query = new GetBasketTotalQuery { BasketId = basketId, IncludeVat = false };

        var total = await _handler.HandleAsync(query);

        total.Should().Be(135.00m);
    }

    [Fact]
    public async Task HandleAsync_ThrowsException_WhenBasketNotFound()
    {
        var query = new GetBasketTotalQuery { BasketId = Guid.NewGuid(), IncludeVat = true };

        var act = async () => await _handler.HandleAsync(query);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

