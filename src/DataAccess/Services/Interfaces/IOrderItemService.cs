using DataAccess.DTOs;
using DataAccess.Models;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface IOrderItemService
	{
		Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);
		Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
		Task CreateOrderItemAsync(OrderItem orderItem);
		Task UpdateOrderItemAsync(OrderItem orderItem);
		Task DeleteOrderItemAsync(int orderItemId);
	}
}
