using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Queries.Handlers;

public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, Basket?>
{
    private readonly BasketDbContext _context;

    public GetBasketQueryHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<Basket?> HandleAsync(GetBasketQuery query, CancellationToken cancellationToken = default)
    {
        return await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == query.BasketId, cancellationToken);
    }
}

