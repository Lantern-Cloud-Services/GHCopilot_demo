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
        /// <summary>
        /// Azure Function to handle HTTP requests for processing orders.
        /// </summary>
        /// <param name="req">The HTTP request containing the order payload.</param>
        /// <param name="log">Logger instance for logging information.</param>
        /// <returns>Returns an IActionResult indicating success or failure.</returns>
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

                    if (order == null || string.IsNullOrEmpty(order.ProductId) || order.Quantity <= 0)
                    {
                        return new BadRequestObjectResult("Invalid order payload.");
                    }

                    // Persist the order to Cosmos DB
                    await OrderPersistence.SaveOrderAsync(order.OrderId, order.ProductId, order.Quantity);

                    return new OkObjectResult(new
                    {
                        message = "Order created successfully",
                        orderId = order.OrderId
                    });
                }
                else if (req.Method == "GET")
                {
                    // Retrieve order by ID
                    string orderId = req.Query["orderId"];

                    if (string.IsNullOrEmpty(orderId))
                    {
                        return new BadRequestObjectResult("Please provide an orderId parameter.");
                    }

                    var order = await OrderPersistence.GetOrderAsync(orderId);

                    if (order == null)
                    {
                        return new NotFoundObjectResult($"Order with ID {orderId} not found.");
                    }

                    return new OkObjectResult(order);
                }

                return new BadRequestObjectResult("Invalid HTTP method.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing request: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}