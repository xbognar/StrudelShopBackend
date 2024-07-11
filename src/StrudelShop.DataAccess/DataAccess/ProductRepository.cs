using Dapper;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess
{
	public class ProductRepository : IProductRepository
	{
		private readonly IDbConnection _dbConnection;

		public ProductRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _dbConnection.QueryAsync<Product>("GetAllProducts", commandType: CommandType.StoredProcedure);
		}

		public async Task<Product> GetProductByIdAsync(int productId)
		{
			return await _dbConnection.QuerySingleOrDefaultAsync<Product>("GetProductById", new { ProductID = productId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> CreateProductAsync(Product product)
		{
			return await _dbConnection.ExecuteAsync("CreateProduct", new { product.Name, product.Description, product.Price, product.ImageUrl }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> UpdateProductAsync(Product product)
		{
			return await _dbConnection.ExecuteAsync("UpdateProduct", new { product.ProductId, product.Name, product.Description, product.Price, product.ImageUrl }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> DeleteProductAsync(int productId)
		{
			return await _dbConnection.ExecuteAsync("DeleteProduct", new { ProductID = productId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync(int top)
		{
			return await _dbConnection.QueryAsync<TopSellingProductDTO>("GetTopSellingProducts", new { Top = top }, commandType: CommandType.StoredProcedure);
		}
	}
}
