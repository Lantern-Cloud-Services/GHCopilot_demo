# Order Processor API

This project includes an Order Processor API that allows you to submit orders to be stored in Cosmos DB.

## Features

- HTTP POST endpoint for submitting orders
- Cosmos DB integration for persistent storage
- Input validation for order data

## API Details

### POST /api/OrderProcessor

Processes an order and stores it in Cosmos DB.

#### Request Body

```json
{
  "orderId": "ORD-123",
  "productId": "PROD-456",
  "quantity": 5
}
```

#### Response

```json
{
  "message": "Order processed successfully",
  "order": {
    "id": "guid-generated-id",
    "orderId": "ORD-123",
    "productId": "PROD-456",
    "quantity": 5,
    "createdAt": "2023-05-20T12:34:56.789Z"
  }
}
```

## Configuration

The API requires the following configuration settings in your `local.settings.json` file:

```json
{
  "Values": {
    "CosmosDbConnectionString": "YOUR_COSMOS_DB_CONNECTION_STRING",
    "CosmosDbName": "OrdersDb",
    "CosmosDbContainer": "Orders"
  }
}
```

## Testing

Use the provided unit tests to verify the functionality of the Order Processor.