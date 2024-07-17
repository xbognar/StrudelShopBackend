using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using StrudelShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;

		public OrderService(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			return await _orderRepository.GetAllOrdersAsync();
		}

		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _orderRepository.GetOrderByIdAsync(orderId);
		}

		public async Task<int> CreateOrderAsync(Order order)
		{
			return await _orderRepository.CreateOrderAsync(order);
		}

		public async Task<int> UpdateOrderAsync(Order order)
		{
			return await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task<int> DeleteOrderAsync(int orderId)
		{
			return await _orderRepository.DeleteOrderAsync(orderId);
		}

		public async Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
		}

		public async Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId)
		{
			return await _orderRepository.GetOrderDetailsAsync(orderId);
		}

		public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _orderRepository.GetTotalSalesByDateRangeAsync(startDate, endDate);
		}
	}
}
