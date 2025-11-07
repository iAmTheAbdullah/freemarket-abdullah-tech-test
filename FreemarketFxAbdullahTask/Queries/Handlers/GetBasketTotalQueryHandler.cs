using FreemarketFxAbdullahTask.Data;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Queries.Handlers;

public class GetBasketTotalQueryHandler : IQueryHandler<GetBasketTotalQuery, decimal>
{
    private readonly BasketDbContext _context;

    public GetBasketTotalQueryHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> HandleAsync(GetBasketTotalQuery query, CancellationToken cancellationToken = default)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == query.BasketId, cancellationToken);

        if (basket == null)
        {
            throw new InvalidOperationException($"Basket with ID {query.BasketId} not found");
        }

        if (query.IncludeVat)
        {
            return Math.Round(basket.GetTotalWithVat(), 2);
        }

        return Math.Round(basket.GetTotalWithoutVat(), 2);
    }
}

