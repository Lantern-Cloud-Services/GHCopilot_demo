using System;
using Newtonsoft.Json;

namespace Company.Function.Models
{
    /// <summary>
    /// Represents an order in the system
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product ordered
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the order was created
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}