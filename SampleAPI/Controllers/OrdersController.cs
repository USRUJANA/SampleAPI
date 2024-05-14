using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;
        // Add more dependencies as needed.

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;

        }
 
        [HttpGet("")] // TODO: Change route, if needed.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrders();
                if (orders == null || orders.Count == 0)
                {
                    return NotFound("No orders found.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting orders.");
            }
        }

        [HttpGet("details/{id:int}", Name = "GetOrderById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                Order? order = await _orderRepository.GetOrderById(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }


        /// TODO: Add an endpoint to allow users to create an order using <see cref="CreateOrderRequest"/>.
        /// 


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = new Order
                {
                    Name = request.Name,
                    Description = request.Description,
                    Invoiced = request.Invoiced ?? true
                };

                Order createdOrder = await _orderRepository.AddNewOrder(order);

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order with details {@OrderRequest}", request);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating order.");
            }
        }

        [HttpGet("recentorders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecentOrders()
        {
            try
            {
                var recentOrders = await _orderRepository.GetRecentOrders();
                if (recentOrders == null || recentOrders.Count == 0)
                {
                    return NotFound("No recent orders found.");
                }
                return Ok(recentOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retrieving recent orders.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                bool deleted = await _orderRepository.DeleteOrder(id);
                if (!deleted)
                {
                    return NotFound("Order not found or already deleted.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
