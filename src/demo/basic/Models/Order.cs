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
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The unique identifier for the product being ordered
        /// </summary>
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// The quantity of the product being ordered
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// The date and time when the order was created
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}