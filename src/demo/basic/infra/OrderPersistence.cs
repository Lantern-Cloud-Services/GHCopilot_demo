using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace BasicDemo
{
    public static class OrderPersistence
    {
        private static readonly string EndpointUri = "<Your-Cosmos-DB-Endpoint>";
        private static readonly string PrimaryKey = "<Your-Cosmos-DB-Key>";
        private static readonly string DatabaseId = "OrdersDatabase";
        private static readonly string ContainerId = "OrdersContainer";

        private static CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

        public static async Task SaveOrderAsync(Order order)
        {
            var container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            await container.CreateItemAsync(order, new PartitionKey(order.OrderId));
        }
    }
}