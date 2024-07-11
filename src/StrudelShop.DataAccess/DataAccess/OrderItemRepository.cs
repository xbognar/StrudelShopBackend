using Dapper;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess
{
	public class OrderItemRepository : IOrderItemRepository
	{
		private readonly IDbConnection _dbConnection;

		public OrderItemRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
		{
			return await _dbConnection.QueryAsync<OrderItem>("GetAllOrderItems", commandType: CommandType.StoredProcedure);
		}

		public async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
		{
			return await _dbConnection.QuerySingleOrDefaultAsync<OrderItem>("GetOrderItemById", new { OrderItemID = orderItemId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> AddOrderItemAsync(OrderItem orderItem)
		{
			return await _dbConnection.ExecuteAsync("AddOrderItem", new { orderItem.OrderId, orderItem.ProductId, orderItem.Quantity, orderItem.Price }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
		{
			return await _dbConnection.ExecuteAsync("UpdateOrderItem", new { orderItem.OrderItemId, orderItem.OrderId, orderItem.ProductId, orderItem.Quantity, orderItem.Price }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> DeleteOrderItemAsync(int orderItemId)
		{
			return await _dbConnection.ExecuteAsync("DeleteOrderItem", new { OrderItemId = orderItemId }, commandType: CommandType.StoredProcedure);
		}
	}
}
