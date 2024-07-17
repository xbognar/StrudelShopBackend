using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using StrudelShop.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;

		public ProductService(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _productRepository.GetAllProductsAsync();
		}

		public async Task<Product> GetProductByIdAsync(int productId)
		{
			return await _productRepository.GetProductByIdAsync(productId);
		}

		public async Task<int> CreateProductAsync(Product product)
		{
			return await _productRepository.CreateProductAsync(product);
		}

		public async Task<int> UpdateProductAsync(Product product)
		{
			return await _productRepository.UpdateProductAsync(product);
		}

		public async Task<int> DeleteProductAsync(int productId)
		{
			return await _productRepository.DeleteProductAsync(productId);
		}

		public async Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync(int top)
		{
			return await _productRepository.GetTopSellingProductsAsync(top);
		}
	}
}
