using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services.Interfaces
{
	public interface IOrderService
	{
		
		Task<IEnumerable<Order>> GetAllOrdersAsync();
		
		Task<Order> GetOrderByIdAsync(int orderId);
		
		Task<int> CreateOrderAsync(Order order);
		
		Task<int> UpdateOrderAsync(Order order);
		
		Task<int> DeleteOrderAsync(int orderId);
		
		Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
		
		Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId);
		
		Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
	
	}
}
