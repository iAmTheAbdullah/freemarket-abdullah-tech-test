namespace FreemarketFxAbdullahTask.Models;

public class DiscountCode
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public decimal Percentage { get; set; }
    public bool IsActive { get; set; }
}

