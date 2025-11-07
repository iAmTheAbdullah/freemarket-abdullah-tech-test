namespace FreemarketFxAbdullahTask.Queries;

public class GetBasketTotalQuery : IQuery<decimal>
{
    public Guid BasketId { get; set; }
    public bool IncludeVat { get; set; }
}

