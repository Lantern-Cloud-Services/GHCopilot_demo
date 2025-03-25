using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Company.Function.Models;
using Company.Function.Repositories;

namespace Company.Function.Tests
{
    /// <summary>
    /// Tests for the CosmosOrderRepository
    /// </summary>
    public class CosmosOrderRepositoryTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<CosmosClient> _cosmosClientMock;
        private readonly Mock<Container> _containerMock;
        private readonly Mock<Database> _databaseMock;

        public CosmosOrderRepositoryTests()
        {
            _loggerMock = new Mock<ILogger>();
            _cosmosClientMock = new Mock<CosmosClient>();
            _containerMock = new Mock<Container>();
            _databaseMock = new Mock<Database>();

            // Setup mocks
            _cosmosClientMock.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_containerMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_Generates_Id_When_Empty()
        {
            // Arrange
            var order = new Order
            {
                ProductId = "product-456",
                Quantity = 3
            };

            _containerMock.Setup(c => c.CreateItemAsync(
                It.IsAny<Order>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order o, PartitionKey pk, ItemRequestOptions ro, CancellationToken ct) => 
                {
                    return Mock.Of<ItemResponse<Order>>(r => r.Resource == o);
                });

            // Use reflection to create repository with mocked CosmosClient
            var constructorInfo = typeof(CosmosOrderRepository).GetConstructor(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null,
                new Type[] { typeof(string), typeof(string), typeof(string), typeof(ILogger) },
                null);

            var repository = (CosmosOrderRepository)constructorInfo.Invoke(
                new object[] { "mock-connection-string", "mock-db", "mock-container", _loggerMock.Object });

            // Use reflection to set the private fields
            var cosmosClientField = typeof(CosmosOrderRepository).GetField("_cosmosClient", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var containerField = typeof(CosmosOrderRepository).GetField("_container", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            cosmosClientField.SetValue(repository, _cosmosClientMock.Object);
            containerField.SetValue(repository, _containerMock.Object);

            // Act
            var result = await repository.CreateOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.Equal(order.ProductId, result.ProductId);
            Assert.Equal(order.Quantity, result.Quantity);
            Assert.NotEqual(default(DateTime), result.CreatedAt);
        }

        [Fact]
        public async Task GetOrderAsync_Returns_Null_When_Not_Found()
        {
            // Arrange
            var orderId = "non-existent-id";

            _containerMock.Setup(c => c.ReadItemAsync<Order>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CosmosException("Not found", System.Net.HttpStatusCode.NotFound, 0, "x", 0));

            // Use reflection to create repository with mocked CosmosClient
            var constructorInfo = typeof(CosmosOrderRepository).GetConstructor(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null,
                new Type[] { typeof(string), typeof(string), typeof(string), typeof(ILogger) },
                null);

            var repository = (CosmosOrderRepository)constructorInfo.Invoke(
                new object[] { "mock-connection-string", "mock-db", "mock-container", _loggerMock.Object });

            // Use reflection to set the private fields
            var cosmosClientField = typeof(CosmosOrderRepository).GetField("_cosmosClient", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var containerField = typeof(CosmosOrderRepository).GetField("_container", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            cosmosClientField.SetValue(repository, _cosmosClientMock.Object);
            containerField.SetValue(repository, _containerMock.Object);

            // Act
            var result = await repository.GetOrderAsync(orderId);

            // Assert
            Assert.Null(result);
        }
    }
}