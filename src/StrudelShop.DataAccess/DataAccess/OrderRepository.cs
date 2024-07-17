using Dapper;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess
{
	public class OrderRepository : IOrderRepository
	{
		private readonly IDbConnection _dbConnection;

		public OrderRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			return await _dbConnection.QueryAsync<Order>("GetAllOrders", commandType: CommandType.StoredProcedure);
		}

		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _dbConnection.QuerySingleOrDefaultAsync<Order>("GetOrderById", new { OrderId = orderId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> CreateOrderAsync(Order order)
		{
			return await _dbConnection.ExecuteAsync("CreateOrder", new { order.CustomerId, order.OrderDate, order.DeliveryDate, order.TotalAmount, order.PaymentStatus }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> UpdateOrderAsync(Order order)
		{
			return await _dbConnection.ExecuteAsync("UpdateOrder", new { order.OrderId, order.CustomerId, order.OrderDate, order.DeliveryDate, order.TotalAmount, order.PaymentStatus }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> DeleteOrderAsync(int orderId)
		{
			return await _dbConnection.ExecuteAsync("DeleteOrder", new { OrderId = orderId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbConnection.QueryAsync<OrderDTO>("GetOrdersByDateRange", new { StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);
		}

		public async Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId)
		{
			return await _dbConnection.QueryFirstOrDefaultAsync<OrderDetailsDTO>("GetOrderDetails", new { OrderId = orderId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbConnection.QueryFirstOrDefaultAsync<decimal>("GetTotalSalesByDateRange", new { StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);
		}
	}
}
