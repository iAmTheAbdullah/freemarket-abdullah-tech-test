using FreemarketFxAbdullahTask.Commands;
using FreemarketFxAbdullahTask.DTOs;
using FreemarketFxAbdullahTask.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FreemarketFxAbdullahTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly ICommandHandler<CreateBasketCommand, Guid> _createBasketHandler;
    private readonly ICommandHandler<AddItemCommand, Guid> _addItemHandler;
    private readonly ICommandHandler<RemoveItemCommand, bool> _removeItemHandler;
    private readonly IQueryHandler<GetBasketQuery, Models.Basket?> _getBasketHandler;

    public BasketController(
        ICommandHandler<CreateBasketCommand, Guid> createBasketHandler,
        ICommandHandler<AddItemCommand, Guid> addItemHandler,
        ICommandHandler<RemoveItemCommand, bool> removeItemHandler,
        IQueryHandler<GetBasketQuery, Models.Basket?> getBasketHandler)
    {
        _createBasketHandler = createBasketHandler;
        _addItemHandler = addItemHandler;
        _removeItemHandler = removeItemHandler;
        _getBasketHandler = getBasketHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBasket(CancellationToken cancellationToken)
    {
        var basketId = await _createBasketHandler.HandleAsync(new CreateBasketCommand(), cancellationToken);
        return Ok(basketId);
    }

    [HttpGet("{basketId}")]
    public async Task<ActionResult<BasketResponse>> GetBasket(Guid basketId, CancellationToken cancellationToken)
    {
        var basket = await _getBasketHandler.HandleAsync(new GetBasketQuery { BasketId = basketId }, cancellationToken);

        if (basket == null)
        {
            return NotFound();
        }

        var response = new BasketResponse
        {
            Id = basket.Id,
            DiscountCode = basket.DiscountCode,
            DiscountPercentage = basket.DiscountPercentage,
            Items = basket.Items.Select(i => new BasketItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity,
                IsDiscounted = i.IsDiscounted,
                DiscountPercentage = i.DiscountPercentage,
                TotalPrice = i.GetTotalPrice()
            }).ToList(),
            Subtotal = basket.GetSubtotal(),
            DiscountAmount = basket.GetDiscountAmount(),
            TotalWithoutVat = basket.GetTotalWithoutVat(),
            TotalWithVat = basket.GetTotalWithVat()
        };

        return Ok(response);
    }

    [HttpPost("{basketId}/items")]
    public async Task<ActionResult<Guid>> AddItem(Guid basketId, [FromBody] AddItemRequest request, CancellationToken cancellationToken)
    {
        var command = new AddItemCommand
        {
            BasketId = basketId,
            ProductName = request.ProductName,
            Price = request.Price,
            Quantity = request.Quantity,
            IsDiscounted = request.IsDiscounted,
            DiscountPercentage = request.DiscountPercentage
        };

        try
        {
            var itemId = await _addItemHandler.HandleAsync(command, cancellationToken);
            return Ok(itemId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{basketId}/items/{itemId}")]
    public async Task<ActionResult> RemoveItem(Guid basketId, Guid itemId, CancellationToken cancellationToken)
    {
        var command = new RemoveItemCommand
        {
            BasketId = basketId,
            ItemId = itemId
        };

        var result = await _removeItemHandler.HandleAsync(command, cancellationToken);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

