using FluentAssertions;
using FluentAssertions.Extensions;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using SampleAPI.Tests.Data;

namespace SampleAPI.Tests.Repositories
{
    public class OrderRepositoryTests : IDisposable
    {
        // TODO: Write repository unit tests
        private readonly SampleApiDbContext _context;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            var dbName = Guid.NewGuid().ToString();
            _context = MockSampleApiDbContextFactory.GenerateMockContext(dbName);
            TestDataSeed.Seed(_context);
            _orderRepository = new OrderRepository(_context);
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturnAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            orders.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder()
        {
            var order = await _orderRepository.GetOrderById(1);
            order.Should().NotBeNull();
            order!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNull()
        {
            var order = await _orderRepository.GetOrderById(99);
            order.Should().BeNull();
        }

        [Fact]
        public async Task GetRecentOrders_ShouldReturnOrdersFromLastDay()
        {
            var orders = await _orderRepository.GetRecentOrders();

            orders.Should().ContainSingle()
                  .And.Subject.First().Id.Should().Be(2);
        }

        [Fact]
        public async Task DeleteOrder_ShouldMarkAsDeleted()
        {
            var result = await _orderRepository.DeleteOrder(1);
            result.Should().BeTrue();
            _context.Orders.First(o => o.Id == 1).Deleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnFalse()
        {
            var result = await _orderRepository.DeleteOrder(99);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddNewOrder_ShouldAddOrderAndSetUniqueId()
        {
            var newOrder = new Order { Name = "New Test Order", Description = "A test description" };

            var addedOrder = await _orderRepository.AddNewOrder(newOrder);

            addedOrder.Should().NotBeNull();
            addedOrder.Id.Should().BePositive("because each new order should be assigned a positive ID");
            var allOrders = await _orderRepository.GetAllOrders();
            allOrders.Should().HaveCount(3);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}