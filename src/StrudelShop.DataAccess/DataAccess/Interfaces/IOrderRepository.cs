using StrudelShop.DataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess.Interfaces
{
	public interface IOrderRepository
	{
		
		Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
		
		Task<OrderDTO> GetOrderByIdAsync(int orderId);
		
		Task<int> CreateOrderAsync(OrderDTO order);
		
		Task<int> UpdateOrderAsync(OrderDTO order);
		
		Task<int> DeleteOrderAsync(int orderId);
		
		Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
		
		Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId);
		
		Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
	
	}

}
