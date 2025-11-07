using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Commands.Handlers;

public class CreateBasketCommandHandler : ICommandHandler<CreateBasketCommand, Guid>
{
    private readonly BasketDbContext _context;

    public CreateBasketCommandHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> HandleAsync(CreateBasketCommand command, CancellationToken cancellationToken = default)
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid()
        };

        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);

        return basket.Id;
    }
}

