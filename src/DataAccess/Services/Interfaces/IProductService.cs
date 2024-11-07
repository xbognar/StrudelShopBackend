using DataAccess.DTOs;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface IProductService
	{
		Task<Product> GetProductByIdAsync(int productId);
		Task<IEnumerable<Product>> GetAllProductsAsync();
		Task CreateProductAsync(Product product);
		Task UpdateProductAsync(Product product);
		Task DeleteProductAsync(int productId);
		Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync();
		Task<IEnumerable<ProductOverviewDTO>> GetProductOverviewAsync();
	}

}
