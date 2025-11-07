using FreemarketFxAbdullahTask.Commands;
using FreemarketFxAbdullahTask.DTOs;
using FreemarketFxAbdullahTask.Queries;
using FreemarketFxAbdullahTask.Services;
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
    private readonly IBasketMapper _mapper;

    public BasketController(
        ICommandHandler<CreateBasketCommand, Guid> createBasketHandler,
        ICommandHandler<AddItemCommand, Guid> addItemHandler,
        ICommandHandler<RemoveItemCommand, bool> removeItemHandler,
        ICommandHandler<ApplyDiscountCodeCommand, bool> applyDiscountHandler,
        IQueryHandler<GetBasketQuery, Models.Basket?> getBasketHandler,
        IQueryHandler<GetBasketTotalQuery, decimal> getTotalHandler,
        IBasketMapper mapper)
    {
        _createBasketHandler = createBasketHandler;
        _addItemHandler = addItemHandler;
        _removeItemHandler = removeItemHandler;
        _applyDiscountHandler = applyDiscountHandler;
        _getBasketHandler = getBasketHandler;
        _getTotalHandler = getTotalHandler;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBasket(CancellationToken cancellationToken)
    {
        var basketId = await _createBasketHandler.HandleAsync(new CreateBasketCommand(), cancellationToken);
        return Ok(basketId);
    }

    [HttpGet("{basketId:guid}")]
    public async Task<ActionResult<BasketResponse>> GetBasket(Guid basketId, CancellationToken cancellationToken)
    {
        var basket = await _getBasketHandler.HandleAsync(new GetBasketQuery { BasketId = basketId }, cancellationToken);

        if (basket == null)
        {
            return NotFound();
        }

        var response = _mapper.MapToResponse(basket);
        return Ok(response);
    }

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
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

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

