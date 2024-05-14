using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public interface IOrderRepository
    {
        // TODO: Create repository methods.

        // Suggestions for repo methods:
        Task<List<Order>> GetRecentOrders();
        Task<Order> AddNewOrder(Order order);

        // Order UpdateOrder(Order order);

        Task<Order?> GetOrderById(int id);
        Task<List<Order>> GetAllOrders();
        Task<bool> DeleteOrder(int id);
    }
}
