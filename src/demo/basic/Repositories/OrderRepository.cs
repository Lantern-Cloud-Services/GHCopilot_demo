using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Company.Function.Models;

namespace Company.Function.Repositories
{
    /// <summary>
    /// Repository for order operations in Cosmos DB
    /// </summary>
    public class OrderRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the OrderRepository class
        /// </summary>
        /// <param name="connectionString">The Cosmos DB connection string</param>
        /// <param name="databaseName">The database name</param>
        /// <param name="containerName">The container name</param>
        /// <param name="logger">The logger</param>
        public OrderRepository(string connectionString, string databaseName, string containerName, ILogger logger)
        {
            _logger = logger;
            _cosmosClient = new CosmosClient(connectionString);
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }

        /// <summary>
        /// Adds an order to the database
        /// </summary>
        /// <param name="order">The order to add</param>
        /// <returns>The added order</returns>
        public async Task<Order> AddOrderAsync(Order order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.Id))
                {
                    order.Id = Guid.NewGuid().ToString();
                }

                ItemResponse<Order> response = await _container.CreateItemAsync(order, new PartitionKey(order.OrderId));
                return response.Resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order to Cosmos DB");
                throw;
            }
        }
    }
}