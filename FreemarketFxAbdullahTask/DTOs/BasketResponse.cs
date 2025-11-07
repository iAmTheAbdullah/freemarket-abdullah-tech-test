namespace FreemarketFxAbdullahTask.DTOs;

public class BasketResponse
{
    public Guid Id { get; set; }
    public List<BasketItemResponse> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalWithoutVat { get; set; }
    public decimal TotalWithVat { get; set; }
}

public class BasketItemResponse
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsDiscounted { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TotalPrice { get; set; }
}

