using DataAccess.DTOs;
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
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Product> GetProductByIdAsync(int productId)
		{
			return await _context.Products.Include(p => p.ProductImages)
				.FirstOrDefaultAsync(p => p.ProductID == productId);
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _context.Products.ToListAsync();
		}

		public async Task CreateProductAsync(Product product)
		{
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateProductAsync(Product product)
		{
			_context.Products.Update(product);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteProductAsync(int productId)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product != null)
			{
				_context.Products.Remove(product);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<TopProductDTO>> GetTopProductsAsync()
		{
		}

		public async Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync()
		{
		}

		public async Task<IEnumerable<ProductOverviewDTO>> GetProductOverviewAsync()
		{
		}
	}

}
