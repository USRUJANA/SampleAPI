using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _context;

        public OrderRepository(SampleApiDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddNewOrder(Order order)
        {
            //order.Id = _nextId++;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders.Where(o => !o.Deleted).ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && !o.Deleted);
        }

        public async Task<List<Order>> GetRecentOrders()
        {
            var now = DateTime.UtcNow;
            return await _context.Orders
                .Where(o => !o.Deleted && (now - o.EntryDate) < TimeSpan.FromDays(1))
                .OrderByDescending(o => o.EntryDate)
                .ToListAsync();
        }
        //public Order? UpdateOrder(Order order)
        //{
        //    var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
        //    if (existingOrder != null)
        //    {
        //        existingOrder.Name = order.Name;
        //        existingOrder.Description = order.Description;
        //        existingOrder.Invoiced = order.Invoiced;
        //        existingOrder.Deleted = order.Deleted;

        //        return existingOrder;
        //    }
        //    return null;
        //}

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && !o.Deleted);
            if (order != null)
            {
                order.Deleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
