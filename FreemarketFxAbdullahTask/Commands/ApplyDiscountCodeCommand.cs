namespace FreemarketFxAbdullahTask.Commands;

public class ApplyDiscountCodeCommand : ICommand<bool>
{
    public Guid BasketId { get; set; }
    public string DiscountCode { get; set; } = string.Empty;
}

