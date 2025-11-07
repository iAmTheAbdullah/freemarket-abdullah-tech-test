using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using FreemarketFxAbdullahTask.Services;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Commands.Handlers;

public class AddItemCommandHandler : ICommandHandler<AddItemCommand, Guid>
{
    private readonly BasketDbContext _context;
    private readonly IValidationService _validationService;

    public AddItemCommandHandler(BasketDbContext context, IValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
    }

    public async Task<Guid> HandleAsync(AddItemCommand command, CancellationToken cancellationToken = default)
    {
        if (!_validationService.ValidateProductName(command.ProductName))
        {
            throw new ArgumentException("Invalid product name");
        }

        if (!_validationService.ValidatePrice(command.Price))
        {
            throw new ArgumentException("Price must be greater than zero");
        }

        if (!_validationService.ValidateQuantity(command.Quantity))
        {
            throw new ArgumentException("Quantity must be greater than zero");
        }

        if (!_validationService.ValidateDiscountPercentage(command.DiscountPercentage))
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100");
        }

        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == command.BasketId, cancellationToken);

        if (basket == null)
        {
            throw new InvalidOperationException($"Basket with ID {command.BasketId} not found");
        }

        var existingItem = basket.Items.FirstOrDefault(i => 
            i.ProductName == command.ProductName && 
            i.IsDiscounted == command.IsDiscounted &&
            i.DiscountPercentage == command.DiscountPercentage);

        if (existingItem != null)
        {
            existingItem.Quantity += command.Quantity;
        }
        else
        {
            var item = new BasketItem
            {
                Id = Guid.NewGuid(),
                BasketId = command.BasketId,
                ProductName = command.ProductName,
                Price = command.Price,
                Quantity = command.Quantity,
                IsDiscounted = command.IsDiscounted,
                DiscountPercentage = command.DiscountPercentage
            };

            basket.Items.Add(item);
            _context.BasketItems.Add(item);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return existingItem?.Id ?? basket.Items.Last().Id;
    }
}

