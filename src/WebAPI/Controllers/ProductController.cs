using DataAccess.DTOs;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
				return NotFound();
			return Ok(product);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
		{
			var products = await _productService.GetAllProductsAsync();
			return Ok(products);
		}

		[HttpPost]
		public async Task<ActionResult> CreateProduct(Product product)
		{
			await _productService.CreateProductAsync(product);
			return CreatedAtAction(nameof(GetProductById), new { id = product.ProductID }, product);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateProduct(int id, Product product)
		{
			if (id != product.ProductID)
				return BadRequest();

			await _productService.UpdateProductAsync(product);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteProduct(int id)
		{
			await _productService.DeleteProductAsync(id);
			return NoContent();
		}

		// Additional endpoints using DTOs
		[HttpGet("top")]
		public async Task<ActionResult<IEnumerable<TopProductDTO>>> GetTopProducts()
		{
			var topProducts = await _productService.GetTopProductsAsync();
			return Ok(topProducts);
		}

		[HttpGet("top-selling")]
		public async Task<ActionResult<IEnumerable<TopSellingProductDTO>>> GetTopSellingProducts()
		{
			var topSellingProducts = await _productService.GetTopSellingProductsAsync();
			return Ok(topSellingProducts);
		}

		[HttpGet("overview")]
		public async Task<ActionResult<IEnumerable<ProductOverviewDTO>>> GetProductOverview()
		{
			var overview = await _productService.GetProductOverviewAsync();
			return Ok(overview);
		}
	}

}
