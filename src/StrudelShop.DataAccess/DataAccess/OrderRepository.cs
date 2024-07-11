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

		public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
		{
			return await _dbConnection.QueryAsync<OrderDTO>("GetAllOrders", commandType: CommandType.StoredProcedure);
		}

		public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
		{
			return await _dbConnection.QuerySingleOrDefaultAsync<OrderDTO>("GetOrderById", new { OrderID = orderId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> CreateOrderAsync(OrderDTO order)
		{
			return await _dbConnection.ExecuteAsync("CreateOrder", new { order.CustomerId, order.OrderDate, order.DeliveryDate, order.TotalAmount, order.PaymentStatus }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> UpdateOrderAsync(OrderDTO order)
		{
			return await _dbConnection.ExecuteAsync("UpdateOrder", new { order.OrderId, order.CustomerId, order.OrderDate, order.DeliveryDate, order.TotalAmount, order.PaymentStatus }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> DeleteOrderAsync(int orderId)
		{
			return await _dbConnection.ExecuteAsync("DeleteOrder", new { OrderID = orderId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbConnection.QueryAsync<OrderDTO>("GetOrdersByDateRange", new { StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);
		}

		public async Task<OrderDetailsDTO> GetOrderDetailsAsync(int orderId)
		{
			var orderDetails = await _dbConnection.QueryFirstOrDefaultAsync<OrderDetailsDTO>("GetOrderDetails", new { OrderID = orderId }, commandType: CommandType.StoredProcedure);
			return orderDetails;
		}

		public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbConnection.QueryFirstOrDefaultAsync<decimal>("GetTotalSalesByDateRange", new { StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);
		}
	}
}
