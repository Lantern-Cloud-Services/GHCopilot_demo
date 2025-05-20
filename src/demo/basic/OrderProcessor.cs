using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Company.Function.Models;
using Company.Function.Repositories;

namespace Company.Function
{
    /// <summary>
    /// HTTP trigger function for processing orders
    /// </summary>
    public static class OrderProcessor
    {
        /// <summary>
        /// Processes an order from an HTTP request
        /// </summary>
        /// <param name="req">The HTTP request</param>
        /// <param name="log">The logger</param>
        /// <returns>An action result</returns>
        [FunctionName("OrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed an order request.");

            try
            {
                // Read the request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                // Deserialize the request body into an Order object
                Order order = JsonConvert.DeserializeObject<Order>(requestBody);
                
                if (order == null || string.IsNullOrEmpty(order.OrderId) || 
                    string.IsNullOrEmpty(order.ProductId) || order.Quantity <= 0)
                {
                    return new BadRequestObjectResult("Invalid order. Please provide orderId, productId, and quantity.");
                }

                // Get Cosmos DB configuration from environment variables
                string connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
                string databaseName = Environment.GetEnvironmentVariable("CosmosDbName");
                string containerName = Environment.GetEnvironmentVariable("CosmosDbContainer");

                // Create a repository and persist the order
                var repository = new OrderRepository(connectionString, databaseName, containerName, log);
                Order savedOrder = await repository.AddOrderAsync(order);

                // Return success response
                return new OkObjectResult(new 
                {
                    message = "Order processed successfully",
                    order = savedOrder
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing order");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}