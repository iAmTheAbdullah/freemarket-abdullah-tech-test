using FreemarketFxAbdullahTask.DTOs;
using FreemarketFxAbdullahTask.Models;

namespace FreemarketFxAbdullahTask.Services;

public interface IBasketMapper
{
    BasketResponse MapToResponse(Basket basket);
}

