using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderItemController : ControllerBase
	{
		private readonly IOrderItemService _orderItemService;

		public OrderItemController(IOrderItemService orderItemService)
		{
			_orderItemService = orderItemService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<OrderItem>> GetOrderItemById(int id)
		{
			var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
			if (orderItem == null)
				return NotFound();
			return Ok(orderItem);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllOrderItems()
		{
			var orderItems = await _orderItemService.GetAllOrderItemsAsync();
			return Ok(orderItems);
		}

		[HttpPost]
		public async Task<ActionResult> CreateOrderItem(OrderItem orderItem)
		{
			await _orderItemService.CreateOrderItemAsync(orderItem);
			return CreatedAtAction(nameof(GetOrderItemById), new { id = orderItem.OrderItemID }, orderItem);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateOrderItem(int id, OrderItem orderItem)
		{
			if (id != orderItem.OrderItemID)
				return BadRequest();

			await _orderItemService.UpdateOrderItemAsync(orderItem);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteOrderItem(int id)
		{
			await _orderItemService.DeleteOrderItemAsync(id);
			return NoContent();
		}
	}

}
