# Shopping Basket API

REST API for an online shopping basket with discount and VAT support.

## Running

```bash
dotnet restore
cd FreemarketFxAbdullahTask
dotnet run
```

API runs on `https://localhost:5001` or `http://localhost:5000`.

## Tests

```bash
dotnet test
```

Test coverage includes unit tests for domain logic, handlers, and services, plus integration tests for full API workflows.

## Implemented

- Create basket
- Add/remove items (same items auto-merge by quantity)
- Item-level discounts
- Basket-level discount codes (applies only to non-discounted items)
- Total with/without 20% VAT
- In-memory EF Core storage

## Not Implemented

Skipped to prioritize architecture and testing:
- Bulk item operations
- Shipping calculations (UK/international)

## Architecture

**CQRS Pattern** - Lightweight implementation without MediatR. Commands mutate state, queries read it. Kept it simple but follows the pattern correctly.

**Service Layer** - Business logic separated into focused services:
- `ValidationService` - input validation
- `DiscountService` - discount calculations and rules
- `VatCalculationService` - VAT logic

**Key Decisions:**
- Discount codes don't stack with item discounts (business rule to prevent double-discounting)
- Decimal rounding to 2 places on all money calculations
- Async throughout for scalability
- In-memory DB for quick setup (would use SQL in prod)

## API

All endpoints return JSON.

### Create Basket
`POST /api/basket` → returns basket GUID

### Get Basket
`GET /api/basket/{id}` → full basket with items and totals

### Add Item
```
POST /api/basket/{id}/items
{
  "productName": "string",
  "price": 10.00,
  "quantity": 1,
  "isDiscounted": false,
  "discountPercentage": 0
}
```

### Remove Item
`DELETE /api/basket/{id}/items/{itemId}`

### Get Totals
- `GET /api/basket/{id}/total` - with VAT
- `GET /api/basket/{id}/total-without-vat` - without VAT

### Apply Discount Code
```
POST /api/basket/{id}/discount-code
{ "discountCode": "SAVE10" }
```

Seeded codes: `SAVE10`, `SAVE20`, `SAVE30`

## Project Structure

```
Commands/         - State mutations (create, add, remove, discount)
Queries/          - Read operations (get basket, calculate totals)
Services/         - Business logic
Models/           - Domain entities
Controllers/      - API endpoints
Tests/Unit/       - Domain, handler, and service tests
Tests/Integration/- Full API scenarios
```

## What I'd Add With More Time

- Proper error responses with problem details
- Optimistic concurrency for basket updates
- Fluent validation for complex rules
- Auth/authz
- Persistent database
- API versioning
