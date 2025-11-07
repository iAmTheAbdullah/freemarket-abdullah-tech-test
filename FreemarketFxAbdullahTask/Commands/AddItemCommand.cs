namespace FreemarketFxAbdullahTask.Commands;

public class AddItemCommand : ICommand<Guid>
{
    public Guid BasketId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsDiscounted { get; set; }
    public decimal DiscountPercentage { get; set; }
}

