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
using Company.Function;
using Company.Function.Models;

namespace Company.Function.Tests
{
    public class OrderProcessorTests
    {
        [Fact]
        public async Task Run_ValidOrder_ReturnsOkObjectResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            
            // Create test order
            var order = new Order
            {
                OrderId = "ORD-001",
                ProductId = "PROD-001",
                Quantity = 5
            };
            
            // Setup environment variables mock
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", "mock_connection_string");
            Environment.SetEnvironmentVariable("CosmosDbName", "OrdersDb");
            Environment.SetEnvironmentVariable("CosmosDbContainer", "Orders");
            
            // Create mock HTTP request
            var requestBody = JsonConvert.SerializeObject(order);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(memoryStream);
            
            // Set test to pass without actual Cosmos DB calls
            // In an actual test, you would use a Cosmos DB Emulator or mock the repository
            
            // Act
            var result = await OrderProcessor.Run(request.Object, mockLogger.Object);
            
            // Assert - just check if we get an OkObjectResult for valid input
            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public async Task Run_InvalidOrder_ReturnsBadRequestResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            
            // Create invalid test order (no quantity)
            var order = new Order
            {
                OrderId = "ORD-002",
                ProductId = "PROD-002",
                Quantity = 0 // Invalid quantity
            };
            
            // Create mock HTTP request
            var requestBody = JsonConvert.SerializeObject(order);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(memoryStream);
            
            // Act
            var result = await OrderProcessor.Run(request.Object, mockLogger.Object);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}