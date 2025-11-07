namespace FreemarketFxAbdullahTask.Services;

public interface IValidationService
{
    bool ValidatePrice(decimal price);
    bool ValidateQuantity(int quantity);
    bool ValidateDiscountPercentage(decimal percentage);
    bool ValidateProductName(string productName);
}

