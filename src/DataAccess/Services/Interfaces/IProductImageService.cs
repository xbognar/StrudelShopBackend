using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface IProductImageService
	{
		Task<ProductImage> GetProductImageByIdAsync(int imageId);
		Task<IEnumerable<ProductImage>> GetAllProductImagesAsync();
		Task CreateProductImageAsync(ProductImage productImage);
		Task UpdateProductImageAsync(ProductImage productImage);
		Task DeleteProductImageAsync(int imageId);
	}
}
