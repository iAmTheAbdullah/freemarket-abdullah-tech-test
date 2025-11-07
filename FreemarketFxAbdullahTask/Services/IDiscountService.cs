using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public interface IDiscountService
{
    decimal CalculateItemDiscount(BasketItem item);
    decimal CalculateBasketDiscount(Basket basket);
    bool CanApplyDiscountCode(BasketItem item);
}

