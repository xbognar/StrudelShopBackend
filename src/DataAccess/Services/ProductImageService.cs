using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Services
{
	public class ProductImageService : IProductImageService
	{
		private readonly ApplicationDbContext _context;

		public ProductImageService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ProductImage> GetProductImageByIdAsync(int imageId)
		{
			return await _context.ProductImages.FindAsync(imageId);
		}

		public async Task<IEnumerable<ProductImage>> GetAllProductImagesAsync()
		{
			return await _context.ProductImages.ToListAsync();
		}

		public async Task CreateProductImageAsync(ProductImage productImage)
		{
			await _context.ProductImages.AddAsync(productImage);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateProductImageAsync(ProductImage productImage)
		{
			_context.ProductImages.Update(productImage);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteProductImageAsync(int imageId)
		{
			var image = await _context.ProductImages.FindAsync(imageId);
			if (image != null)
			{
				_context.ProductImages.Remove(image);
				await _context.SaveChangesAsync();
			}
		}
	}
}
