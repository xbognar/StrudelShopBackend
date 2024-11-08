using DataAccess.DTOs;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
		{
			var orders = await _orderService.GetAllOrdersAsync();
			return Ok(orders);
		}

		[Authorize(Roles = "User,Admin")]
		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrderById(int id)
		{
			var order = await _orderService.GetOrderByIdAsync(id);
			if (order == null)
				return NotFound();
			return Ok(order);
		}

		[Authorize(Roles = "User")]
		[HttpPost]
		public async Task<ActionResult> CreateOrder(Order order)
		{
			await _orderService.CreateOrderAsync(order);
			return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, order);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateOrder(int id, Order order)
		{
			if (id != order.OrderID)
				return BadRequest();

			await _orderService.UpdateOrderAsync(order);
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteOrder(int id)
		{
			await _orderService.DeleteOrderAsync(id);
			return NoContent();
		}

		[Authorize(Roles = "User")]
		[HttpGet("history/{userId}")]
		public async Task<ActionResult<IEnumerable<OrderHistoryDTO>>> GetOrderHistory(int userId)
		{
			var orderHistory = await _orderService.GetOrderHistoryAsync(userId);
			return Ok(orderHistory);
		}

		[Authorize(Roles = "User,Admin")]
		[HttpGet("details/{id}")]
		public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int id)
		{
			var orderDetails = await _orderService.GetOrderDetailsAsync(id);
			return Ok(orderDetails);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("summary")]
		public async Task<ActionResult<IEnumerable<CustomerOrderSummaryDTO>>> GetCustomerOrderSummaries()
		{
			var summaries = await _orderService.GetCustomerOrderSummariesAsync();
			return Ok(summaries);
		}
	}
}
