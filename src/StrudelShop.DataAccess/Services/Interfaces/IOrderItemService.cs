using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services.Interfaces
{
	public interface IOrderItemService
	{
		
		Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
		
		Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);
		
		Task<int> AddOrderItemAsync(OrderItem orderItem);
		
		Task<int> UpdateOrderItemAsync(OrderItem orderItem);
		
		Task<int> DeleteOrderItemAsync(int orderItemId);
	
	}
}
