namespace FreemarketFxAbdullahTask.Models;

public class BasketItem
{
    private const decimal PercentageDivisor = 100m;

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
            return Math.Round(basePrice * (1 - DiscountPercentage / PercentageDivisor), 2);
        }
        return Math.Round(basePrice, 2);
    }
}

