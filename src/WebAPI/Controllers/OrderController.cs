using DataAccess.DTOs;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrderById(int id)
		{
			var order = await _orderService.GetOrderByIdAsync(id);
			if (order == null)
				return NotFound();
			return Ok(order);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
		{
			var orders = await _orderService.GetAllOrdersAsync();
			return Ok(orders);
		}

		[HttpPost]
		public async Task<ActionResult> CreateOrder(Order order)
		{
			await _orderService.CreateOrderAsync(order);
			return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, order);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateOrder(int id, Order order)
		{
			if (id != order.OrderID)
				return BadRequest();

			await _orderService.UpdateOrderAsync(order);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteOrder(int id)
		{
			await _orderService.DeleteOrderAsync(id);
			return NoContent();
		}

		// Additional endpoints using DTOs
		[HttpGet("history/{userId}")]
		public async Task<ActionResult<IEnumerable<OrderHistoryDTO>>> GetOrderHistory(int userId)
		{
			var orderHistory = await _orderService.GetOrderHistoryAsync(userId);
			return Ok(orderHistory);
		}

		[HttpGet("details/{id}")]
		public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int id)
		{
			var orderDetails = await _orderService.GetOrderDetailsAsync(id);
			return Ok(orderDetails);
		}

		[HttpGet("summary")]
		public async Task<ActionResult<IEnumerable<CustomerOrderSummaryDTO>>> GetCustomerOrderSummaries()
		{
			var summaries = await _orderService.GetCustomerOrderSummariesAsync();
			return Ok(summaries);
		}
	}

}
