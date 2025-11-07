namespace FreemarketFxAbdullahTask.Models;

public class DiscountCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public bool IsActive { get; set; }
}

