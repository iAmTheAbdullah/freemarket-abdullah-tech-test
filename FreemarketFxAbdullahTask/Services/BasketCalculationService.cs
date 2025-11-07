using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public class BasketCalculationService : IBasketCalculationService
{
    private readonly IVatCalculationService _vatService;
    private const decimal PercentageDivisor = 100m;

    public BasketCalculationService(IVatCalculationService vatService)
    {
        _vatService = vatService;
    }

    public decimal CalculateItemTotal(BasketItem item)
    {
        var basePrice = item.Price * item.Quantity;
        
        if (item.IsDiscounted)
        {
            var discount = basePrice * (item.DiscountPercentage / PercentageDivisor);
            return Math.Round(basePrice - discount, 2);
        }
        
        return Math.Round(basePrice, 2);
    }

    public decimal CalculateSubtotal(Basket basket)
    {
        return Math.Round(basket.Items.Sum(CalculateItemTotal), 2);
    }

    public decimal CalculateDiscountAmount(Basket basket)
    {
        var eligibleItemsTotal = basket.Items
            .Where(item => !item.IsDiscounted)
            .Sum(CalculateItemTotal);
        
        return Math.Round(eligibleItemsTotal * (basket.DiscountPercentage / PercentageDivisor), 2);
    }

    public decimal CalculateTotalWithoutVat(Basket basket)
    {
        return Math.Round(CalculateSubtotal(basket) - CalculateDiscountAmount(basket), 2);
    }

    public decimal CalculateTotalWithVat(Basket basket)
    {
        var totalWithoutVat = CalculateTotalWithoutVat(basket);
        return _vatService.AddVat(totalWithoutVat);
    }
}

