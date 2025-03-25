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
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing a new order.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);

            if (order == null || string.IsNullOrEmpty(order.OrderId) || string.IsNullOrEmpty(order.ProductId) || order.Quantity <= 0)
            {
                return new BadRequestObjectResult("Invalid order payload.");
            }

            await OrderPersistence.SaveOrderAsync(order);

            return new OkObjectResult(new { message = "Order saved successfully.", order });
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}