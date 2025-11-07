using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FreemarketFxAbdullahTask.DTOs;

namespace FreemarketFxAbdullahTask.Tests.Integration;

public class BasketApiTests : IClassFixture<BasketApiTestFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly BasketApiTestFactory _factory;

    public BasketApiTests(BasketApiTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    [Fact]
    public async Task CreateBasket_ReturnsGuidId()
    {
        var response = await _client.PostAsync("/api/basket", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basketId = await response.Content.ReadFromJsonAsync<Guid>();
        basketId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetBasket_ReturnsBasketDetails()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/basket/{basketId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var basket = await getResponse.Content.ReadFromJsonAsync<BasketResponse>();
        basket.Should().NotBeNull();
        basket!.Id.Should().Be(basketId);
    }

    [Fact]
    public async Task AddItem_ThenGetBasket_ShowsItem()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var addItemRequest = new AddItemRequest
        {
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 2,
            IsDiscounted = false
        };

        var addResponse = await _client.PostAsJsonAsync($"/api/basket/{basketId}/items", addItemRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync($"/api/basket/{basketId}");
        var basket = await getResponse.Content.ReadFromJsonAsync<BasketResponse>();

        basket!.Items.Should().HaveCount(1);
        basket.Items[0].ProductName.Should().Be("Test Product");
        basket.Items[0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task CalculateTotal_WithVat_ReturnsCorrectAmount()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var addItemRequest = new AddItemRequest
        {
            ProductName = "Test Product",
            Price = 100.00m,
            Quantity = 1,
            IsDiscounted = false
        };

        await _client.PostAsJsonAsync($"/api/basket/{basketId}/items", addItemRequest);

        var totalResponse = await _client.GetAsync($"/api/basket/{basketId}/total");
        var total = await totalResponse.Content.ReadFromJsonAsync<decimal>();

        total.Should().Be(120.00m);
    }

    [Fact]
    public async Task CalculateTotal_WithoutVat_ReturnsCorrectAmount()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var addItemRequest = new AddItemRequest
        {
            ProductName = "Test Product",
            Price = 100.00m,
            Quantity = 1,
            IsDiscounted = false
        };

        await _client.PostAsJsonAsync($"/api/basket/{basketId}/items", addItemRequest);

        var totalResponse = await _client.GetAsync($"/api/basket/{basketId}/total-without-vat");
        var total = await totalResponse.Content.ReadFromJsonAsync<decimal>();

        total.Should().Be(100.00m);
    }

    [Fact]
    public async Task ApplyDiscountCode_ReducesTotal()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var addItemRequest = new AddItemRequest
        {
            ProductName = "Test Product",
            Price = 100.00m,
            Quantity = 1,
            IsDiscounted = false
        };

        await _client.PostAsJsonAsync($"/api/basket/{basketId}/items", addItemRequest);

        var discountRequest = new ApplyDiscountCodeRequest { DiscountCode = "SAVE10" };
        var discountResponse = await _client.PostAsJsonAsync($"/api/basket/{basketId}/discount-code", discountRequest);
        discountResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var totalResponse = await _client.GetAsync($"/api/basket/{basketId}/total-without-vat");
        var total = await totalResponse.Content.ReadFromJsonAsync<decimal>();

        total.Should().Be(90.00m);
    }

    [Fact]
    public async Task RemoveItem_RemovesFromBasket()
    {
        var createResponse = await _client.PostAsync("/api/basket", null);
        var basketId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var addItemRequest = new AddItemRequest
        {
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 1,
            IsDiscounted = false
        };

        var addResponse = await _client.PostAsJsonAsync($"/api/basket/{basketId}/items", addItemRequest);
        var itemId = await addResponse.Content.ReadFromJsonAsync<Guid>();

        var removeResponse = await _client.DeleteAsync($"/api/basket/{basketId}/items/{itemId}");
        removeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/basket/{basketId}");
        var basket = await getResponse.Content.ReadFromJsonAsync<BasketResponse>();

        basket!.Items.Should().BeEmpty();
    }
}

