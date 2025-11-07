namespace FreemarketFxAbdullahTask.Models;

public class Basket
{
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    public decimal DiscountPercentage { get; set; }

    public decimal GetSubtotal()
    {
        return Math.Round(Items.Sum(item => item.GetTotalPrice()), 2);
    }

    public decimal GetDiscountAmount()
    {
        var eligibleItems = Items.Where(item => !item.IsDiscounted).Sum(item => item.GetTotalPrice());
        return Math.Round(eligibleItems * (DiscountPercentage / 100), 2);
    }

    public decimal GetTotalWithoutVat()
    {
        return Math.Round(GetSubtotal() - GetDiscountAmount(), 2);
    }

    public decimal GetTotalWithVat()
    {
        var totalWithoutVat = GetTotalWithoutVat();
        return Math.Round(totalWithoutVat * 1.20m, 2);
    }
}

