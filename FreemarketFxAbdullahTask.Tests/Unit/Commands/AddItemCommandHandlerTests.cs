using FluentAssertions;
using FreemarketFxAbdullahTask.Commands;
using FreemarketFxAbdullahTask.Commands.Handlers;
using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using FreemarketFxAbdullahTask.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FreemarketFxAbdullahTask.Tests.Unit.Commands;

public class AddItemCommandHandlerTests
{
    private readonly BasketDbContext _context;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly AddItemCommandHandler _handler;

    public AddItemCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<BasketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new BasketDbContext(options);

        _validationServiceMock = new Mock<IValidationService>();
        _validationServiceMock.Setup(v => v.ValidateProductName(It.IsAny<string>())).Returns(true);
        _validationServiceMock.Setup(v => v.ValidatePrice(It.IsAny<decimal>())).Returns(true);
        _validationServiceMock.Setup(v => v.ValidateQuantity(It.IsAny<int>())).Returns(true);
        _validationServiceMock.Setup(v => v.ValidateDiscountPercentage(It.IsAny<decimal>())).Returns(true);

        _handler = new AddItemCommandHandler(_context, _validationServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_AddsNewItemToBasket()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket { Id = basketId };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var command = new AddItemCommand
        {
            BasketId = basketId,
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 2,
            IsDiscounted = false
        };

        var itemId = await _handler.HandleAsync(command);

        itemId.Should().NotBeEmpty();
        var updatedBasket = await _context.Baskets.Include(b => b.Items).FirstAsync(b => b.Id == basketId);
        updatedBasket.Items.Should().HaveCount(1);
        updatedBasket.Items[0].ProductName.Should().Be("Test Product");
    }

    [Fact]
    public async Task HandleAsync_ThrowsException_WhenBasketNotFound()
    {
        var command = new AddItemCommand
        {
            BasketId = Guid.NewGuid(),
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 2
        };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task HandleAsync_IncreasesQuantity_WhenSameItemExists()
    {
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = basketId,
            Items = new List<BasketItem>
            {
                new() { Id = Guid.NewGuid(), BasketId = basketId, ProductName = "Test Product", Price = 10.00m, Quantity = 1, IsDiscounted = false }
            }
        };
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();

        var command = new AddItemCommand
        {
            BasketId = basketId,
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 2,
            IsDiscounted = false
        };

        await _handler.HandleAsync(command);

        var updatedBasket = await _context.Baskets.Include(b => b.Items).FirstAsync(b => b.Id == basketId);
        updatedBasket.Items.Should().HaveCount(1);
        updatedBasket.Items[0].Quantity.Should().Be(3);
    }
}

