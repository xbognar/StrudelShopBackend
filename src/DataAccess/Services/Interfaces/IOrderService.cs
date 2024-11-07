using DataAccess.DTOs;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface IOrderService
	{
		Task<Order> GetOrderByIdAsync(int orderId);
		Task<IEnumerable<Order>> GetAllOrdersAsync();
		Task CreateOrderAsync(Order order);
		Task UpdateOrderAsync(Order order);
		Task DeleteOrderAsync(int orderId);

		Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(int userId);
		Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId);
		Task<IEnumerable<CustomerOrderSummaryDTO>> GetCustomerOrderSummariesAsync();
	}

}
