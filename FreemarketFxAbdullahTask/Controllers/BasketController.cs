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
    private readonly ICommandHandler<ApplyDiscountCodeCommand, bool> _applyDiscountHandler;
    private readonly IQueryHandler<GetBasketQuery, Models.Basket?> _getBasketHandler;
    private readonly IQueryHandler<GetBasketTotalQuery, decimal> _getTotalHandler;

    public BasketController(
        ICommandHandler<CreateBasketCommand, Guid> createBasketHandler,
        ICommandHandler<AddItemCommand, Guid> addItemHandler,
        ICommandHandler<RemoveItemCommand, bool> removeItemHandler,
        ICommandHandler<ApplyDiscountCodeCommand, bool> applyDiscountHandler,
        IQueryHandler<GetBasketQuery, Models.Basket?> getBasketHandler,
        IQueryHandler<GetBasketTotalQuery, decimal> getTotalHandler)
    {
        _createBasketHandler = createBasketHandler;
        _addItemHandler = addItemHandler;
        _removeItemHandler = removeItemHandler;
        _applyDiscountHandler = applyDiscountHandler;
        _getBasketHandler = getBasketHandler;
        _getTotalHandler = getTotalHandler;
    }

    /// <summary>
    /// Creates a new shopping basket
    /// </summary>
    /// <returns>The ID of the newly created basket</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBasket(CancellationToken cancellationToken)
    {
        var basketId = await _createBasketHandler.HandleAsync(new CreateBasketCommand(), cancellationToken);
        return Ok(basketId);
    }

    /// <summary>
    /// Retrieves a basket by its ID
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <returns>The basket details including all items and totals</returns>
    [HttpGet("{basketId:guid}")]
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

    /// <summary>
    /// Adds an item to the basket. If the same item already exists, quantity is increased
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <param name="request">Item details including price, quantity, and discount info</param>
    /// <returns>The ID of the added or updated item</returns>
    [HttpPost("{basketId:guid}/items")]
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

    /// <summary>
    /// Removes an item from the basket
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <param name="itemId">The item ID to remove</param>
    [HttpDelete("{basketId:guid}/items/{itemId:guid}")]
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

    /// <summary>
    /// Gets the total cost of the basket including 20% VAT
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <returns>Total cost with VAT applied</returns>
    [HttpGet("{basketId:guid}/total")]
    public async Task<ActionResult<decimal>> GetTotalWithVat(Guid basketId, CancellationToken cancellationToken)
    {
        try
        {
            var total = await _getTotalHandler.HandleAsync(new GetBasketTotalQuery
            {
                BasketId = basketId,
                IncludeVat = true
            }, cancellationToken);

            return Ok(total);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets the total cost of the basket without VAT
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <returns>Total cost without VAT</returns>
    [HttpGet("{basketId:guid}/total-without-vat")]
    public async Task<ActionResult<decimal>> GetTotalWithoutVat(Guid basketId, CancellationToken cancellationToken)
    {
        try
        {
            var total = await _getTotalHandler.HandleAsync(new GetBasketTotalQuery
            {
                BasketId = basketId,
                IncludeVat = false
            }, cancellationToken);

            return Ok(total);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Applies a discount code to the basket. Only applies to non-discounted items
    /// </summary>
    /// <param name="basketId">The basket ID</param>
    /// <param name="request">Discount code to apply</param>
    [HttpPost("{basketId:guid}/discount-code")]
    public async Task<ActionResult> ApplyDiscountCode(Guid basketId, [FromBody] ApplyDiscountCodeRequest request, CancellationToken cancellationToken)
    {
        var command = new ApplyDiscountCodeCommand
        {
            BasketId = basketId,
            DiscountCode = request.DiscountCode
        };

        var result = await _applyDiscountHandler.HandleAsync(command, cancellationToken);

        if (!result)
        {
            return NotFound("Basket or discount code not found");
        }

        return Ok();
    }
}

