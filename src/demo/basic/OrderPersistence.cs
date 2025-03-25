using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Company.Function
{
    public static class OrderPersistence
    {
        private static readonly string EndpointUri = "<your-cosmosdb-endpoint-uri>";
        private static readonly string PrimaryKey = "<your-cosmosdb-primary-key>";
        private static readonly string DatabaseId = "OrdersDatabase";
        private static readonly string ContainerId = "OrdersContainer";

        private static CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

        /// <summary>
        /// Handles persistence of orders to Cosmos DB.
        /// </summary>
        /// <param name="orderId">The unique identifier for the order.</param>
        /// <param name="productId">The unique identifier for the product.</param>
        /// <param name="quantity">The quantity of the product ordered.</param>
        public static async Task SaveOrderAsync(string orderId, string productId, int quantity)
        {
            var container = cosmosClient.GetContainer(DatabaseId, ContainerId);

            var order = new
            {
                id = orderId,
                productId = productId,
                quantity = quantity,
                orderDate = DateTime.UtcNow
            };

            await container.CreateItemAsync(order, new PartitionKey(orderId));
        }
    }
}