using FreemarketFxAbdullahTask.Data;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Commands.Handlers;

public class ApplyDiscountCodeCommandHandler : ICommandHandler<ApplyDiscountCodeCommand, bool>
{
    private readonly BasketDbContext _context;

    public ApplyDiscountCodeCommandHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(ApplyDiscountCodeCommand command, CancellationToken cancellationToken = default)
    {
        var basket = await _context.Baskets
            .FirstOrDefaultAsync(b => b.Id == command.BasketId, cancellationToken);

        if (basket == null)
        {
            return false;
        }

        var discountCode = await _context.DiscountCodes
            .FirstOrDefaultAsync(dc => dc.Code == command.DiscountCode && dc.IsActive, cancellationToken);

        if (discountCode == null)
        {
            return false;
        }

        basket.DiscountCode = discountCode.Code;
        basket.DiscountPercentage = discountCode.Percentage;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

