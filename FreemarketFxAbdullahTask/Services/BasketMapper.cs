using FreemarketFxAbdullahTask.DTOs;
using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public class BasketMapper : IBasketMapper
{
    private readonly IBasketCalculationService _calculationService;

    public BasketMapper(IBasketCalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public BasketResponse MapToResponse(Basket basket)
    {
        return new BasketResponse
        {
            Id = basket.Id,
            DiscountCode = basket.DiscountCode,
            DiscountPercentage = basket.DiscountPercentage,
            Items = basket.Items.Select(MapItemToResponse).ToList(),
            Subtotal = _calculationService.CalculateSubtotal(basket),
            DiscountAmount = _calculationService.CalculateDiscountAmount(basket),
            TotalWithoutVat = _calculationService.CalculateTotalWithoutVat(basket),
            TotalWithVat = _calculationService.CalculateTotalWithVat(basket)
        };
    }

    private BasketItemResponse MapItemToResponse(BasketItem item)
    {
        return new BasketItemResponse
        {
            Id = item.Id,
            ProductName = item.ProductName,
            Price = item.Price,
            Quantity = item.Quantity,
            IsDiscounted = item.IsDiscounted,
            DiscountPercentage = item.DiscountPercentage,
            TotalPrice = _calculationService.CalculateItemTotal(item)
        };
    }
}

