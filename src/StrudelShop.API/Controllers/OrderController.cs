using Microsoft.AspNetCore.Mvc;
using StrudelShop.Services.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace StrudelShop.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class OrdersController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrdersController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		// GET: api/orders
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
		{
			var orders = await _orderService.GetAllOrdersAsync();
			return Ok(orders);
		}

		// GET: api/orders/{orderId}
		[HttpGet("{orderId}")]
		public async Task<ActionResult<Order>> GetOrderById(int orderId)
		{
			var order = await _orderService.GetOrderByIdAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}

		// POST: api/orders
		[HttpPost]
		public async Task<ActionResult> CreateOrder(Order order)
		{
			await _orderService.CreateOrderAsync(order);
			return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
		}

		// PUT: api/orders/{orderId}
		[HttpPut("{orderId}")]
		public async Task<ActionResult> UpdateOrder(int orderId, Order order)
		{
			if (orderId != order.OrderId)
			{
				return BadRequest();
			}

			await _orderService.UpdateOrderAsync(order);
			return NoContent();
		}

		// DELETE: api/orders/{orderId}
		[HttpDelete("{orderId}")]
		public async Task<ActionResult> DeleteOrder(int orderId)
		{
			await _orderService.DeleteOrderAsync(orderId);
			return NoContent();
		}

		// GET: api/orders/by-date-range
		[HttpGet("by-date-range")]
		public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var orders = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
			return Ok(orders);
		}

		// GET: api/orders/{orderId}/details
		[HttpGet("{orderId}/details")]
		public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
		{
			var orderDetails = await _orderService.GetOrderDetailsAsync(orderId);
			return Ok(orderDetails);
		}

		// GET: api/orders/total-sales
		[HttpGet("total-sales")]
		public async Task<ActionResult<decimal>> GetTotalSalesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var totalSales = await _orderService.GetTotalSalesByDateRangeAsync(startDate, endDate);
			return Ok(totalSales);
		}
	}
}
