using DataAccess.DTOs;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
	public class OrderService : IOrderService
	{
		private readonly ApplicationDbContext _context;

		public OrderService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _context.Orders.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
				.FirstOrDefaultAsync(o => o.OrderID == orderId);
		}

		public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			return await _context.Orders.ToListAsync();
		}

		public async Task CreateOrderAsync(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateOrderAsync(Order order)
		{
			_context.Orders.Update(order);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteOrderAsync(int orderId)
		{
			var order = await _context.Orders.FindAsync(orderId);
			if (order != null)
			{
				_context.Orders.Remove(order);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(int userId)
		{
			// Logic to map orders to OrderHistoryDTO for a specific user
		}

		public async Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId)
		{
			// Logic to map a single order to OrderDetailsDTO
		}

		public async Task<IEnumerable<CustomerOrderSummaryDTO>> GetCustomerOrderSummariesAsync()
		{
			// Logic to map orders to CustomerOrderSummaryDTO for admin view
		}
	}

}
