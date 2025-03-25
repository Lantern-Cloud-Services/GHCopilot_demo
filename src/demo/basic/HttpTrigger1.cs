using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string orderId = data?.orderId;
            string productId = data?.productId;
            int quantity = data?.quantity;

            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(productId) || quantity <= 0)
            {
                return new BadRequestObjectResult("Invalid order payload.");
            }

            // Persist the order to Cosmos DB
            await OrderPersistence.SaveOrderAsync(orderId, productId, quantity);

            return new OkObjectResult($"Order received. Order ID: {orderId}, Product ID: {productId}, Quantity: {quantity}");
        }
    }
}