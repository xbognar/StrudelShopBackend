using DataAccess.DTOs;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.Services.Interfaces;

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
			return await _context.Orders
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
				.FirstOrDefaultAsync(o => o.OrderID == orderId);
		}

		public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			return await _context.Orders.Include(o => o.OrderItems).ToListAsync();
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
			return await _context.Orders
				.Where(o => o.UserID == userId)
				.Select(o => new OrderHistoryDTO
				{
					OrderId = o.OrderID,
					OrderDate = o.OrderDate,
					DeliveryDate = o.DeliveryDate,
					TotalAmount = o.TotalAmount,
					PaymentStatus = o.PaymentStatus
				})
				.ToListAsync();
		}

		public async Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId)
		{
			var order = await _context.Orders
				.Include(o => o.User)
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
				.FirstOrDefaultAsync(o => o.OrderID == orderId);

			if (order == null) return null;

			return new OrderDetailsDTO
			{
				OrderId = order.OrderID,
				OrderDate = order.OrderDate,
				DeliveryDate = order.DeliveryDate,
				OrderTotalAmount = order.TotalAmount,
				PaymentStatus = order.PaymentStatus,
				CustomerFirstName = order.User.FirstName,
				CustomerLastName = order.User.LastName,
				CustomerEmail = order.User.Email,
				CustomerPhoneNumber = order.User.PhoneNumber,
				CustomerAddress = order.User.Address,
				OrderItems = order.OrderItems.ToList() 
			};
		}

		public async Task<IEnumerable<CustomerOrderSummaryDTO>> GetCustomerOrderSummariesAsync()
		{
			return await _context.Orders
				.Include(o => o.User)
				.Select(o => new CustomerOrderSummaryDTO
				{
					OrderId = o.OrderID,
					OrderDate = o.OrderDate,
					TotalAmount = o.TotalAmount,
					CustomerName = $"{o.User.FirstName} {o.User.LastName}",
					PaymentStatus = o.PaymentStatus
				})
				.ToListAsync();
		}
	}
}
