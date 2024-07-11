using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess.Interfaces
{
	public interface IProductRepository
	{
		
		Task<IEnumerable<Product>> GetAllProductsAsync();
		
		Task<Product> GetProductByIdAsync(int productId);
		
		Task<int> CreateProductAsync(Product product);
		
		Task<int> UpdateProductAsync(Product product);
		
		Task<int> DeleteProductAsync(int productId);
		
		Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync(int top);
	
	}

}
