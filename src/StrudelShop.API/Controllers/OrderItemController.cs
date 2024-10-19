using Microsoft.AspNetCore.Mvc;
using StrudelShop.Services.Interfaces;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace StrudelShop.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class OrderItemsController : ControllerBase
	{
		private readonly IOrderItemService _orderItemService;

		public OrderItemsController(IOrderItemService orderItemService)
		{
			_orderItemService = orderItemService;
		}

		// GET: api/orderitems
		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllOrderItems()
		{
			var orderItems = await _orderItemService.GetAllOrderItemsAsync();
			return Ok(orderItems);
		}

		// GET: api/orderitems/{orderItemId}
		[HttpGet("{orderItemId}")]
		public async Task<ActionResult<OrderItem>> GetOrderItemById(int orderItemId)
		{
			var orderItem = await _orderItemService.GetOrderItemByIdAsync(orderItemId);
			if (orderItem == null)
			{
				return NotFound();
			}
			return Ok(orderItem);
		}

		// POST: api/orderitems
		[HttpPost]
		public async Task<ActionResult> AddOrderItem(OrderItem orderItem)
		{
			await _orderItemService.AddOrderItemAsync(orderItem);
			return CreatedAtAction(nameof(GetOrderItemById), new { orderItemId = orderItem.OrderItemId }, orderItem);
		}

		// PUT: api/orderitems/{orderItemId}
		[HttpPut("{orderItemId}")]
		public async Task<ActionResult> UpdateOrderItem(int orderItemId, OrderItem orderItem)
		{
			if (orderItemId != orderItem.OrderItemId)
			{
				return BadRequest();
			}

			await _orderItemService.UpdateOrderItemAsync(orderItem);
			return NoContent();
		}

		// DELETE: api/orderitems/{orderItemId}
		[HttpDelete("{orderItemId}")]
		public async Task<ActionResult> DeleteOrderItem(int orderItemId)
		{
			await _orderItemService.DeleteOrderItemAsync(orderItemId);
			return NoContent();
		}
	}
}
