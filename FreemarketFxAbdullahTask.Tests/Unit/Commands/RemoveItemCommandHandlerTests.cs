using FluentAssertions;
using FreemarketFxAbdullahTask.Commands;
using FreemarketFxAbdullahTask.Commands.Handlers;
using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Tests.Unit.Commands;

public class RemoveItemCommandHandlerTests
{
    private readonly BasketDbContext _context;
    private readonly RemoveItemCommandHandler _handler;

    public RemoveItemCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<BasketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new BasketDbContext(options);

        _handler = new RemoveItemCommandHandler(_context);
    }

    [Fact]
    public async Task HandleAsync_RemovesItemFromBasket()
    {
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = basketId,
            Items = new List<BasketItem>
            {
                new() { Id = itemId, BasketId = basketId, ProductName = "Test Product", Price = 10.00m, Quantity = 1 }
            }
        };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var command = new RemoveItemCommand { BasketId = basketId, ItemId = itemId };

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        var updatedBasket = await _context.Baskets.Include(b => b.Items).FirstAsync(b => b.Id == basketId);
        updatedBasket.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFalse_WhenBasketNotFound()
    {
        var command = new RemoveItemCommand { BasketId = Guid.NewGuid(), ItemId = Guid.NewGuid() };

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFalse_WhenItemNotFound()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket { Id = basketId };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var command = new RemoveItemCommand { BasketId = basketId, ItemId = Guid.NewGuid() };

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
    }
}

