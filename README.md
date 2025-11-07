# Shopping Basket API

REST API for an online shopping basket with discount and VAT support.

## Running

```bash
dotnet restore
cd FreemarketFxAbdullahTask
dotnet run
```

API runs on `http://localhost:5000`.

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

## API Usage

Use curl or Postman to test endpoints. Examples below assume `http://localhost:5000`.

### Create Basket
```bash
curl -X POST http://localhost:5000/api/basket -k
# Returns: "3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

### Add Item
```bash
curl -X POST http://localhost:5000/api/basket/{basketId}/items -k \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Laptop",
    "price": 999.99,
    "quantity": 1,
    "isDiscounted": false,
    "discountPercentage": 0
  }'
```

### Add Discounted Item
```bash
curl -X POST http://localhost:5000/api/basket/{basketId}/items -k \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Mouse",
    "price": 25.00,
    "quantity": 2,
    "isDiscounted": true,
    "discountPercentage": 15
  }'
```

### Apply Discount Code
```bash
curl -X POST http://localhost:5000/api/basket/{basketId}/discount-code -k \
  -H "Content-Type: application/json" \
  -d '{ "discountCode": "SAVE10" }'
```

Seeded codes: `SAVE10`, `SAVE20`, `SAVE30`

### Get Basket Details
```bash
curl http://localhost:5000/api/basket/{basketId} -k
```

### Get Totals
```bash
# With VAT
curl http://localhost:5000/api/basket/{basketId}/total -k

# Without VAT
curl http://localhost:5000/api/basket/{basketId}/total-without-vat -k
```

### Remove Item
```bash
curl -X DELETE http://localhost:5000/api/basket/{basketId}/items/{itemId} -k
```

Note: `-k` flag bypasses SSL cert validation for local dev.

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
