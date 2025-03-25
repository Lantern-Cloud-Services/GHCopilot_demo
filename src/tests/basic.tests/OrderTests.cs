using System;
using Xunit;
using Company.Function.Models;
using Newtonsoft.Json;

namespace Company.Function.Tests
{
    /// <summary>
    /// Tests for the Order model
    /// </summary>
    public class OrderTests
    {
        [Fact]
        public void Order_Serialization_Works_Correctly()
        {
            // Arrange
            var order = new Order
            {
                Id = "test-id-123",
                ProductId = "product-456",
                Quantity = 5,
                CreatedAt = new DateTime(2025, 3, 25, 10, 30, 0, DateTimeKind.Utc)
            };

            // Act
            var json = JsonConvert.SerializeObject(order);
            var deserializedOrder = JsonConvert.DeserializeObject<Order>(json);

            // Assert
            Assert.NotNull(deserializedOrder);
            Assert.Equal(order.Id, deserializedOrder.Id);
            Assert.Equal(order.ProductId, deserializedOrder.ProductId);
            Assert.Equal(order.Quantity, deserializedOrder.Quantity);
            Assert.Equal(order.CreatedAt, deserializedOrder.CreatedAt);
        }

        [Fact]
        public void Order_Properties_Set_Correctly()
        {
            // Arrange & Act
            var order = new Order
            {
                Id = "order-123",
                ProductId = "product-456",
                Quantity = 10,
                CreatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("order-123", order.Id);
            Assert.Equal("product-456", order.ProductId);
            Assert.Equal(10, order.Quantity);
            Assert.NotEqual(default(DateTime), order.CreatedAt);
        }
    }
}