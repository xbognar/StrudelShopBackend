using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductImageController : ControllerBase
	{
		private readonly IProductImageService _productImageService;

		public ProductImageController(IProductImageService productImageService)
		{
			_productImageService = productImageService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductImage>>> GetAllProductImages()
		{
			var productImages = await _productImageService.GetAllProductImagesAsync();
			return Ok(productImages);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductImage>> GetProductImageById(int id)
		{
			var productImage = await _productImageService.GetProductImageByIdAsync(id);
			if (productImage == null)
				return NotFound();
			return Ok(productImage);
		}

		[HttpPost]
		public async Task<ActionResult> CreateProductImage(ProductImage productImage)
		{
			await _productImageService.CreateProductImageAsync(productImage);
			return CreatedAtAction(nameof(GetProductImageById), new { id = productImage.ImageID }, productImage);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateProductImage(int id, ProductImage productImage)
		{
			if (id != productImage.ImageID)
				return BadRequest();

			await _productImageService.UpdateProductImageAsync(productImage);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteProductImage(int id)
		{
			await _productImageService.DeleteProductImageAsync(id);
			return NoContent();
		}
	}
}
