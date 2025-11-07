using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Queries;

public class GetBasketQuery : IQuery<Basket?>
{
    public Guid BasketId { get; set; }
}

