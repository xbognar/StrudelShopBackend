using Microsoft.AspNetCore.Mvc;
using StrudelShop.Services.Interfaces;
using StrudelShop.DataAccess.DTOs;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
		{
			var products = await _productService.GetAllProductsAsync();
			return Ok(products);
		}

		[HttpGet("{productId}")]
		public async Task<ActionResult<Product>> GetProductById(int productId)
		{
			var product = await _productService.GetProductByIdAsync(productId);
			if (product == null)
			{
				return NotFound();
			}
			return Ok(product);
		}

		[HttpPost]
		public async Task<ActionResult> CreateProduct(Product product)
		{
			await _productService.CreateProductAsync(product);
			return CreatedAtAction(nameof(GetProductById), new { productId = product.ProductId }, product);
		}

		[HttpPut("{productId}")]
		public async Task<ActionResult> UpdateProduct(int productId, Product product)
		{
			if (productId != product.ProductId)
			{
				return BadRequest();
			}

			await _productService.UpdateProductAsync(product);
			return NoContent();
		}

		[HttpDelete("{productId}")]
		public async Task<ActionResult> DeleteProduct(int productId)
		{
			await _productService.DeleteProductAsync(productId);
			return NoContent();
		}

		[HttpGet("top-selling")]
		public async Task<ActionResult<IEnumerable<TopSellingProductDTO>>> GetTopSellingProducts([FromQuery] int top)
		{
			var topSellingProducts = await _productService.GetTopSellingProductsAsync(top);
			return Ok(topSellingProducts);
		}
	}
}
