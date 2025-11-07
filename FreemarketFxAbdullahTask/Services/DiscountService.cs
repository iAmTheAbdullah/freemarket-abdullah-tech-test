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
        return Math.Round(basePrice * (item.DiscountPercentage / 100), 2);
    }

    public decimal CalculateBasketDiscount(Basket basket)
    {
        var eligibleItems = basket.Items.Where(item => !item.IsDiscounted);
        var eligibleTotal = eligibleItems.Sum(item => item.Price * item.Quantity);
        return Math.Round(eligibleTotal * (basket.DiscountPercentage / 100), 2);
    }

    public bool CanApplyDiscountCode(BasketItem item)
    {
        return !item.IsDiscounted;
    }
}

