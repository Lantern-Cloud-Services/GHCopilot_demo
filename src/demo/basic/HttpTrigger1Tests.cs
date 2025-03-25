using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace Company.Function.Tests
{
    public class HttpTrigger1Tests
    {
        [Fact]
        public async Task Run_ValidOrder_ReturnsOk()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new { orderId = "123", productId = "456", quantity = 10 })
            ));

            // Act
            var result = await HttpTrigger1.Run(request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Order received", okResult.Value.ToString());
        }

        [Fact]
        public async Task Run_InvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new { orderId = "", productId = "", quantity = 0 })
            ));

            // Act
            var result = await HttpTrigger1.Run(request, loggerMock.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}