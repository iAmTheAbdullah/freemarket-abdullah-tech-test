namespace FreemarketFxAbdullahTask.Models;

public class Basket
{
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    public decimal DiscountPercentage { get; set; }

    public decimal GetSubtotal()
    {
        return Items.Sum(item => item.GetTotalPrice());
    }

    public decimal GetDiscountAmount()
    {
        var eligibleItems = Items.Where(item => !item.IsDiscounted).Sum(item => item.GetTotalPrice());
        return eligibleItems * (DiscountPercentage / 100);
    }

    public decimal GetTotalWithoutVat()
    {
        return GetSubtotal() - GetDiscountAmount();
    }

    public decimal GetTotalWithVat()
    {
        var totalWithoutVat = GetTotalWithoutVat();
        return totalWithoutVat * 1.20m;
    }
}

