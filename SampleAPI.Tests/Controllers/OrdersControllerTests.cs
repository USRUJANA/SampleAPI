using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        // TODO: Write controller unit tests
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mockOrderRepository.Object, _mockLogger.Object);
        }
        [Fact]
        public async Task GetOrders_ShouldReturn200OK_WhenOrdersExist()
        {
            var orders = new List<Order>
        {
            new Order { Id = 1, Name = "Order1" },
            new Order { Id = 2, Name = "Order2" }
        };
            _mockOrderRepository.Setup(repo => repo.GetAllOrders()).ReturnsAsync(orders);
            var result = await _controller.GetOrders();
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(orders);
        }

        [Fact]
        public async Task GetOrders_ShouldReturn404NotFound_WhenNoOrdersExist()
        {
            _mockOrderRepository.Setup(repo => repo.GetAllOrders()).ReturnsAsync(new List<Order>());
            var result = await _controller.GetOrders();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetOrderById_ShouldReturn200OK_WhenOrderExists()
        {
            var order = new Order { Id = 1, Name = "Order1" };
            _mockOrderRepository.Setup(repo => repo.GetOrderById(1)).ReturnsAsync(order);
            var result = await _controller.GetOrderById(1);
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(order);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturn404NotFound_WhenOrderDoesNotExist()
        {
            _mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).ReturnsAsync(value: null);
            var result = await _controller.GetOrderById(999);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateOrder_ShouldReturn201Created_WhenValidRequest()
        { 
            var newOrder = new Order { Name = "New Order", Description = "Description", Invoiced = true };
            _mockOrderRepository.Setup(repo => repo.AddNewOrder(It.IsAny<Order>())).ReturnsAsync(newOrder);
            var result = await _controller.CreateOrder(new CreateOrderRequest { Name = "New Order", Description = "Description", Invoiced = true });
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.Value.Should().BeEquivalentTo(newOrder);
            createdResult.ActionName.Should().Be("GetOrderById");
        }

        [Fact]
        public async Task GetRecentOrders_NoRecentOrders_ShouldReturnNotFound()
        {
            _mockOrderRepository.Setup(repo => repo.GetRecentOrders()).ReturnsAsync(new List<Order>());
            var result = await _controller.GetRecentOrders();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No recent orders found.", notFoundResult.Value);
        }
        [Fact]
        public async Task GetRecentOrders_RecentOrdersFound_ShouldReturnOkWithOrders()
        {
            var orders = new List<Order>
            {
                new() { Id = 1, Name = "Order1", Description = "Description1", Invoiced = true },
                new() { Id = 2, Name = "Order2", Description = "Description2", Invoiced = false }
            };
            _mockOrderRepository.Setup(repo => repo.GetRecentOrders()).ReturnsAsync(orders);
            var result = await _controller.GetRecentOrders();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
            Assert.Equal(orders.Count, returnedOrders.Count);
            Assert.Equal(orders, returnedOrders);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturn204NoContent_WhenOrderDeleted()
        {
            _mockOrderRepository.Setup(repo => repo.DeleteOrder(1)).ReturnsAsync(true);
            var result = await _controller.DeleteOrder(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturn404NotFound_WhenOrderDoesNotExist()
        {
            _mockOrderRepository.Setup(repo => repo.DeleteOrder(It.IsAny<int>())).ReturnsAsync(false);
            var result = await _controller.DeleteOrder(999);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}
