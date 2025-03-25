using System.Threading.Tasks;
using Company.Function.Models;

namespace Company.Function.Repositories
{
    /// <summary>
    /// Interface for order repository operations
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Creates a new order in the repository
        /// </summary>
        /// <param name="order">The order to create</param>
        /// <returns>The created order</returns>
        Task<Order> CreateOrderAsync(Order order);

        /// <summary>
        /// Gets an order by its ID
        /// </summary>
        /// <param name="id">The ID of the order to retrieve</param>
        /// <returns>The order if found, null otherwise</returns>
        Task<Order> GetOrderAsync(string id);
    }
}