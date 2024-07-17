using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Models;
using StrudelShop.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services
{
	public class OrderItemService : IOrderItemService
	{
		private readonly IOrderItemRepository _orderItemRepository;

		public OrderItemService(IOrderItemRepository orderItemRepository)
		{
			_orderItemRepository = orderItemRepository;
		}

		public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
		{
			return await _orderItemRepository.GetAllOrderItemsAsync();
		}

		public async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
		{
			return await _orderItemRepository.GetOrderItemByIdAsync(orderItemId);
		}

		public async Task<int> AddOrderItemAsync(OrderItem orderItem)
		{
			return await _orderItemRepository.AddOrderItemAsync(orderItem);
		}

		public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
		{
			return await _orderItemRepository.UpdateOrderItemAsync(orderItem);
		}

		public async Task<int> DeleteOrderItemAsync(int orderItemId)
		{
			return await _orderItemRepository.DeleteOrderItemAsync(orderItemId);
		}
	}
}
