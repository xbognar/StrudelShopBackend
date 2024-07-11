using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess.Interfaces
{
	public interface IOrderItemRepository
	{
		
		Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
		
		Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);
		
		Task<int> AddOrderItemAsync(OrderItem orderItem);
		
		Task<int> UpdateOrderItemAsync(OrderItem orderItem);
		
		Task<int> DeleteOrderItemAsync(int orderItemId);
	
	}

}
