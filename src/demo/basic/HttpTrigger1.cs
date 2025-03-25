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
    /// HTTP Trigger function for processing orders
    /// </summary>
    public static class HttpTrigger1
    {
        // CosmosDB configuration
        private static readonly string CosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
        private static readonly string DatabaseName = Environment.GetEnvironmentVariable("DatabaseName") ?? "OrdersDb";
        private static readonly string ContainerName = Environment.GetEnvironmentVariable("ContainerName") ?? "Orders";

        /// <summary>
        /// Processes an incoming order request
        /// </summary>
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (req.Method == "POST")
                {
                    // Parse the order from the request body
                    var order = JsonConvert.DeserializeObject<Order>(requestBody);
                    
                    if (order == null)
                    {
                        return new BadRequestObjectResult("Invalid order payload");
                    }
                    
                    if (string.IsNullOrEmpty(order.ProductId))
                    {
                        return new BadRequestObjectResult("ProductId is required");
                    }
                    
                    if (order.Quantity <= 0)
                    {
                        return new BadRequestObjectResult("Quantity must be greater than zero");
                    }
                    
                    // Create order repository
                    var orderRepository = CreateOrderRepository(log);
                    
                    // Persist the order to CosmosDB
                    var createdOrder = await orderRepository.CreateOrderAsync(order);
                    
                    return new OkObjectResult(new 
                    { 
                        message = "Order created successfully", 
                        order = createdOrder 
                    });
                }
                else if (req.Method == "GET")
                {
                    // Retrieve order by ID
                    string orderId = req.Query["orderId"];
                    
                    if (string.IsNullOrEmpty(orderId))
                    {
                        return new BadRequestObjectResult("Please provide an orderId parameter");
                    }
                    
                    var orderRepository = CreateOrderRepository(log);
                    var order = await orderRepository.GetOrderAsync(orderId);
                    
                    if (order == null)
                    {
                        return new NotFoundObjectResult($"Order with ID {orderId} not found");
                    }
                    
                    return new OkObjectResult(order);
                }
                
                return new BadRequestObjectResult("Invalid HTTP method");
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing request: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Creates an instance of the order repository
        /// </summary>
        private static IOrderRepository CreateOrderRepository(ILogger log)
        {
            if (string.IsNullOrEmpty(CosmosDbConnectionString))
            {
                throw new InvalidOperationException("CosmosDbConnectionString environment variable is not set");
            }
            
            return new CosmosOrderRepository(CosmosDbConnectionString, DatabaseName, ContainerName, log);
        }
    }
}