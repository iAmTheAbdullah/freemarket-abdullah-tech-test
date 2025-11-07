using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public interface IBasketCalculationService
{
    decimal CalculateItemTotal(BasketItem item);
    decimal CalculateSubtotal(Basket basket);
    decimal CalculateDiscountAmount(Basket basket);
    decimal CalculateTotalWithoutVat(Basket basket);
    decimal CalculateTotalWithVat(Basket basket);
}

