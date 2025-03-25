using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace BasicDemo.Tests
{
    public class HttpTrigger1Tests
    {
        [Fact]
        public async Task Run_ValidOrder_ReturnsOkResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var order = new Order { OrderId = "123", ProductId = "456", Quantity = 10 };
            var requestBody = JsonConvert.SerializeObject(order);
            var request = new DefaultHttpContext().Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            // Act
            var result = await HttpTrigger1.Run(request, mockLogger.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Order saved successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task Run_InvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var requestBody = "{ \"invalid\": \"payload\" }";
            var request = new DefaultHttpContext().Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            // Act
            var result = await HttpTrigger1.Run(request, mockLogger.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}