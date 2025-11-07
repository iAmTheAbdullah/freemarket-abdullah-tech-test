using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Commands.Handlers;

public class AddItemCommandHandler : ICommandHandler<AddItemCommand, Guid>
{
    private readonly BasketDbContext _context;

    public AddItemCommandHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> HandleAsync(AddItemCommand command, CancellationToken cancellationToken = default)
    {
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

