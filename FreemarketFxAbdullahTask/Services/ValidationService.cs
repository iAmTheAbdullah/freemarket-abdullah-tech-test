namespace FreemarketFxAbdullahTask.Services;

public class ValidationService : IValidationService
{
    public bool ValidatePrice(decimal price)
    {
        return price > 0;
    }

    public bool ValidateQuantity(int quantity)
    {
        return quantity > 0;
    }

    public bool ValidateDiscountPercentage(decimal percentage)
    {
        return percentage >= 0 && percentage <= 100;
    }

    public bool ValidateProductName(string productName)
    {
        return !string.IsNullOrWhiteSpace(productName) && productName.Length <= 200;
    }
}

