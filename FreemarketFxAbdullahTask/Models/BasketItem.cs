namespace FreemarketFxAbdullahTask.Models;

public class BasketItem
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsDiscounted { get; set; }
    public decimal DiscountPercentage { get; set; }

    public decimal GetTotalPrice()
    {
        var basePrice = Price * Quantity;
        if (IsDiscounted)
        {
            return basePrice * (1 - DiscountPercentage / 100);
        }
        return basePrice;
    }
}

