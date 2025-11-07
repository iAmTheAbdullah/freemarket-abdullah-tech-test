using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Services;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Queries.Handlers;

public class GetBasketTotalQueryHandler : IQueryHandler<GetBasketTotalQuery, decimal>
{
    private readonly BasketDbContext _context;
    private readonly IBasketCalculationService _calculationService;

    public GetBasketTotalQueryHandler(BasketDbContext context, IBasketCalculationService calculationService)
    {
        _context = context;
        _calculationService = calculationService;
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

        return query.IncludeVat 
            ? _calculationService.CalculateTotalWithVat(basket)
            : _calculationService.CalculateTotalWithoutVat(basket);
    }
}

