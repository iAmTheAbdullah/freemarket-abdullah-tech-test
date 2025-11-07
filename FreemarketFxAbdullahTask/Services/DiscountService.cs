using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public class DiscountService : IDiscountService
{
    public decimal CalculateItemDiscount(BasketItem item)
    {
        if (!item.IsDiscounted)
        {
            return 0;
        }

        var basePrice = item.Price * item.Quantity;
        return basePrice * (item.DiscountPercentage / 100);
    }

    public decimal CalculateBasketDiscount(Basket basket)
    {
        var eligibleItems = basket.Items.Where(item => !item.IsDiscounted);
        var eligibleTotal = eligibleItems.Sum(item => item.Price * item.Quantity);
        return eligibleTotal * (basket.DiscountPercentage / 100);
    }

    public bool CanApplyDiscountCode(BasketItem item)
    {
        return !item.IsDiscounted;
    }
}

