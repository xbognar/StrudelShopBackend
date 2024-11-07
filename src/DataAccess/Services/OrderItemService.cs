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
	public class OrderItemService : IOrderItemService
	{
		private readonly ApplicationDbContext _context;

		public OrderItemService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
		{
			return await _context.OrderItems.FindAsync(orderItemId);
		}

		public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
		{
			return await _context.OrderItems.ToListAsync();
		}

		public async Task CreateOrderItemAsync(OrderItem orderItem)
		{
			await _context.OrderItems.AddAsync(orderItem);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateOrderItemAsync(OrderItem orderItem)
		{
			_context.OrderItems.Update(orderItem);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteOrderItemAsync(int orderItemId)
		{
			var orderItem = await _context.OrderItems.FindAsync(orderItemId);
			if (orderItem != null)
			{
				_context.OrderItems.Remove(orderItem);
				await _context.SaveChangesAsync();
			}
		}
	}
}
