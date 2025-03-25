using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Company.Function.Models;

namespace Company.Function.Repositories
{
    /// <summary>
    /// CosmosDB implementation of the order repository
    /// </summary>
    public class CosmosOrderRepository : IOrderRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for CosmosOrderRepository
        /// </summary>
        /// <param name="connectionString">CosmosDB connection string</param>
        /// <param name="databaseName">CosmosDB database name</param>
        /// <param name="containerName">CosmosDB container name</param>
        /// <param name="logger">Logger instance</param>
        public CosmosOrderRepository(string connectionString, string databaseName, string containerName, ILogger logger)
        {
            _cosmosClient = new CosmosClient(connectionString);
            _container = _cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.Id))
                {
                    order.Id = Guid.NewGuid().ToString();
                }

                order.CreatedAt = DateTime.UtcNow;
                
                ItemResponse<Order> response = await _container.CreateItemAsync(
                    order,
                    new PartitionKey(order.Id));

                _logger.LogInformation($"Order created successfully. Order ID: {order.Id}");
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Failed to create order. Status code: {ex.StatusCode}, Message: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Order> GetOrderAsync(string id)
        {
            try
            {
                ItemResponse<Order> response = await _container.ReadItemAsync<Order>(
                    id,
                    new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Order with ID {id} not found");
                return null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Failed to get order. Status code: {ex.StatusCode}, Message: {ex.Message}");
                throw;
            }
        }
    }
}