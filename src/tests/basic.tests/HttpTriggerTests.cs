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
using Company.Function.Repositories;

namespace Company.Function.Tests
{
    /// <summary>
    /// Tests for the HttpTrigger1 function
    /// </summary>
    public class HttpTriggerTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;

        public HttpTriggerTests()
        {
            _loggerMock = new Mock<ILogger>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
        }

        [Fact]
        public async Task Post_ValidOrder_Returns_OkResult()
        {
            // Arrange
            var order = new Order
            {
                ProductId = "product-123",
                Quantity = 5
            };

            var requestBody = JsonConvert.SerializeObject(order);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);
            var requestBodyStream = new MemoryStream(requestBodyBytes);

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns("POST");
            request.Setup(r => r.Body).Returns(requestBodyStream);

            _orderRepositoryMock.Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) => {
                    o.Id = "generated-id";
                    o.CreatedAt = DateTime.UtcNow;
                    return o;
                });

            // Use environment variable reflection to mock repository creation
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", "mock-connection-string");

            // Act
            var result = await HttpTrigger1.Run(request.Object, _loggerMock.Object);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            
            dynamic responseContent = okResult.Value;
            Assert.Equal("Order created successfully", responseContent.message.ToString());
            Assert.NotNull(responseContent.order);
            
            // Clean up
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", null);
        }

        [Fact]
        public async Task Post_InvalidOrder_Returns_BadRequest()
        {
            // Arrange
            var order = new Order
            {
                ProductId = "", // Invalid - empty product ID
                Quantity = 5
            };

            var requestBody = JsonConvert.SerializeObject(order);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);
            var requestBodyStream = new MemoryStream(requestBodyBytes);

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns("POST");
            request.Setup(r => r.Body).Returns(requestBodyStream);

            // Act
            var result = await HttpTrigger1.Run(request.Object, _loggerMock.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("ProductId is required", badRequestResult.Value);
        }

        [Fact]
        public async Task Get_ExistingOrder_Returns_OkResult()
        {
            // Arrange
            var orderId = "existing-order-id";
            var existingOrder = new Order
            {
                Id = orderId,
                ProductId = "product-456",
                Quantity = 10,
                CreatedAt = DateTime.UtcNow
            };

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns("GET");
            request.Setup(r => r.Query).Returns(new QueryCollection(
                new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "orderId", new Microsoft.Extensions.Primitives.StringValues(orderId) }
                }));

            _orderRepositoryMock.Setup(r => r.GetOrderAsync(orderId))
                .ReturnsAsync(existingOrder);

            // Use environment variable reflection to mock repository creation
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", "mock-connection-string");

            // Act
            var result = await HttpTrigger1.Run(request.Object, _loggerMock.Object);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var returnedOrder = (Order)okResult.Value;
            
            Assert.Equal(orderId, returnedOrder.Id);
            Assert.Equal(existingOrder.ProductId, returnedOrder.ProductId);
            Assert.Equal(existingOrder.Quantity, returnedOrder.Quantity);
            
            // Clean up
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", null);
        }

        [Fact]
        public async Task Get_NonExistingOrder_Returns_NotFound()
        {
            // Arrange
            var orderId = "non-existing-order-id";

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns("GET");
            request.Setup(r => r.Query).Returns(new QueryCollection(
                new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "orderId", new Microsoft.Extensions.Primitives.StringValues(orderId) }
                }));

            _orderRepositoryMock.Setup(r => r.GetOrderAsync(orderId))
                .ReturnsAsync((Order)null);

            // Use environment variable reflection to mock repository creation
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", "mock-connection-string");

            // Act
            var result = await HttpTrigger1.Run(request.Object, _loggerMock.Object);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal($"Order with ID {orderId} not found", notFoundResult.Value);
            
            // Clean up
            Environment.SetEnvironmentVariable("CosmosDbConnectionString", null);
        }
    }
}