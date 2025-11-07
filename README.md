# Shopping Basket API

A REST API for managing an online shopping basket with support for items, discounts, and VAT calculations.

## What's Implemented

- Create a new basket
- Add items to basket (with automatic quantity merging for identical items)
- Remove items from basket
- Add discounted items with custom discount percentages
- Apply discount codes (which only affect non-discounted items)
- Calculate total cost with 20% VAT
- Calculate total cost without VAT
- In-memory data storage using EF Core
- Comprehensive unit and integration tests

## What's Not Implemented

Due to time constraints, I prioritized core functionality and code quality over implementing all features:

- Bulk add multiple items at once (could be added easily by accepting a list in the controller)
- UK shipping cost calculation
- International shipping cost calculation

These features were deprioritized to focus on proper architecture, testing, and ensuring the core basket operations work correctly.

## Architecture

The solution follows SOLID principles and uses several patterns:

### CQRS (Command Query Responsibility Segregation)
I implemented a lightweight CQRS pattern without using MediatR or other heavy frameworks. Commands handle state changes (creating baskets, adding/removing items) while Queries handle read operations (getting basket details, calculating totals). This separation makes the code easier to test and maintain.

The key interfaces are:
- `ICommand<TResult>` and `ICommandHandler<TCommand, TResult>`
- `IQuery<TResult>` and `IQueryHandler<TQuery, TResult>`

### Service Layer
Business logic is separated into focused services following the Single Responsibility Principle:
- `ValidationService` - validates input data (prices, quantities, discount percentages)
- `DiscountService` - handles discount calculations and rules
- `VatCalculationService` - manages VAT calculations

### Dependency Injection
All dependencies are registered in `ServiceCollectionExtensions` for clean service configuration.

### Data Access
Using Entity Framework Core with in-memory database for quick setup. The `BasketDbContext` manages all entities and relationships.

## Project Structure

```
FreemarketFxAbdullahTask/
  Commands/           - Command definitions and handlers
  Queries/            - Query definitions and handlers
  Controllers/        - API endpoints
  Models/             - Domain entities
  Services/           - Business logic services
  Data/               - Database context
  DTOs/               - Request/response objects
  Extensions/         - Service registration extensions

FreemarketFxAbdullahTask.Tests/
  Unit/               - Unit tests for models, handlers, and services
  Integration/        - End-to-end API tests
```

## Prerequisites

- .NET 8.0 SDK

## Setup and Running

1. Clone the repository

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
cd FreemarketFxAbdullahTask
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`).

4. Access Swagger UI:
Open `https://localhost:5001/swagger` in your browser to see the interactive API documentation.

## Running Tests

Run all tests (unit + integration):
```bash
dotnet test
```

Run with detailed output:
```bash
dotnet test --verbosity normal
```

The test suite includes:
- **Unit tests**: Domain models, command handlers, query handlers, and services
- **Integration tests**: Full end-to-end API scenarios

## API Endpoints

### Create Basket
`POST /api/basket`

Returns a GUID for the new basket.

### Get Basket
`GET /api/basket/{basketId}`

Returns basket details including all items and calculated totals.

### Add Item
`POST /api/basket/{basketId}/items`

Request body:
```json
{
  "productName": "Product Name",
  "price": 10.50,
  "quantity": 2,
  "isDiscounted": false,
  "discountPercentage": 0
}
```

If adding the same item (same name and discount settings), the quantity is increased automatically.

### Remove Item
`DELETE /api/basket/{basketId}/items/{itemId}`

Removes an item from the basket.

### Get Total with VAT
`GET /api/basket/{basketId}/total`

Returns the total cost including 20% VAT.

### Get Total without VAT
`GET /api/basket/{basketId}/total-without-vat`

Returns the total cost excluding VAT.

### Apply Discount Code
`POST /api/basket/{basketId}/discount-code`

Request body:
```json
{
  "discountCode": "SAVE10"
}
```

Available discount codes (seeded on startup):
- SAVE10 (10% off)
- SAVE20 (20% off)
- SAVE30 (30% off)

Note: Discount codes only apply to items that don't already have item-level discounts.

## Example Usage

1. Create a basket:
```bash
curl -X POST https://localhost:5001/api/basket
```

2. Add a regular item:
```bash
curl -X POST https://localhost:5001/api/basket/{basketId}/items \
  -H "Content-Type: application/json" \
  -d '{"productName":"Laptop","price":999.99,"quantity":1,"isDiscounted":false,"discountPercentage":0}'
```

3. Add a discounted item:
```bash
curl -X POST https://localhost:5001/api/basket/{basketId}/items \
  -H "Content-Type: application/json" \
  -d '{"productName":"Mouse","price":25.00,"quantity":2,"isDiscounted":true,"discountPercentage":15}'
```

4. Apply a discount code:
```bash
curl -X POST https://localhost:5001/api/basket/{basketId}/discount-code \
  -H "Content-Type: application/json" \
  -d '{"discountCode":"SAVE10"}'
```

5. Get total with VAT:
```bash
curl https://localhost:5001/api/basket/{basketId}/total
```

## Design Decisions

- **In-memory database**: Quick to set up and perfect for this demo. In production, would use SQL Server or PostgreSQL.
- **Lightweight CQRS**: Didn't want to add MediatR dependency for this size of project. The pattern is clear without the overhead.
- **Decimal rounding**: All money values are rounded to 2 decimal places to avoid floating point issues.
- **Discount rules**: Basket-wide discount codes intentionally exclude items that already have item-level discounts to prevent double-discounting.
- **Async/await**: Used throughout for scalability, even though in-memory operations are fast.

## Time Spent

Completed in approximately 4 hours, focusing on:
- Clean architecture and SOLID principles
- Comprehensive test coverage
- Clear separation of concerns
- Production-ready code quality

## Known Limitations

- No authentication/authorization (would add in production)
- No persistence between restarts (in-memory database)
- No concurrent basket modification handling (would add optimistic concurrency)
- Limited input validation (would expand in production)

