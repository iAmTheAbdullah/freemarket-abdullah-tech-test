using FreemarketFxAbdullahTask.Data;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Commands.Handlers;

public class RemoveItemCommandHandler : ICommandHandler<RemoveItemCommand, bool>
{
    private readonly BasketDbContext _context;

    public RemoveItemCommandHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(RemoveItemCommand command, CancellationToken cancellationToken = default)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == command.BasketId, cancellationToken);

        if (basket == null)
        {
            return false;
        }

        var item = basket.Items.FirstOrDefault(i => i.Id == command.ItemId);
        if (item == null)
        {
            return false;
        }

        basket.Items.Remove(item);
        _context.BasketItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

