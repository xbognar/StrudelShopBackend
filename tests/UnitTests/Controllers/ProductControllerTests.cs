using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.DTOs;
using StrudelShop.DataAccess.Services.Interfaces;
using WebAPI.Controllers;

namespace UnitTests.Controllers
{
	public class ProductControllerTests
	{
		private readonly Mock<IProductService> _productServiceMock;
		private readonly ProductController _controller;

		public ProductControllerTests()
		{
			_productServiceMock = new Mock<IProductService>();
			_controller = new ProductController(_productServiceMock.Object);
		}

		[Fact]
		public async Task GetAllProducts_ReturnsOkWithList()
		{
			// Arrange
			var products = new List<Product>
			{
				new Product { ProductID = 1, Name = "Apple Strudel" },
				new Product { ProductID = 2, Name = "Cherry Strudel" }
			};
			_productServiceMock.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

			// Act
			var result = await _controller.GetAllProducts();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<Product>>(okResult.Value);
			returnedList.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetProductById_WhenFound_ReturnsOk()
		{
			// Arrange
			var product = new Product { ProductID = 10, Name = "Test Product" };
			_productServiceMock.Setup(s => s.GetProductByIdAsync(10)).ReturnsAsync(product);

			// Act
			var result = await _controller.GetProductById(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedProduct = Assert.IsType<Product>(okResult.Value);
			returnedProduct.ProductID.Should().Be(10);
		}

		[Fact]
		public async Task GetProductById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_productServiceMock.Setup(s => s.GetProductByIdAsync(999)).ReturnsAsync((Product)null);

			// Act
			var result = await _controller.GetProductById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task CreateProduct_ReturnsCreatedAtAction()
		{
			// Arrange
			var newProduct = new Product { ProductID = 5, Name = "New Product" };
			_productServiceMock.Setup(s => s.CreateProductAsync(newProduct)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateProduct(newProduct);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newProduct);
		}

		[Fact]
		public async Task UpdateProduct_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var product = new Product { ProductID = 2 };
			_productServiceMock.Setup(s => s.UpdateProductAsync(product)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateProduct(2, product);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateProduct_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var product = new Product { ProductID = 2 };

			// Act
			var result = await _controller.UpdateProduct(999, product);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task DeleteProduct_ReturnsNoContent()
		{
			// Arrange
			_productServiceMock.Setup(s => s.DeleteProductAsync(3)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteProduct(3);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task GetTopSellingProducts_ReturnsOk()
		{
			// Arrange
			var topProducts = new List<TopSellingProductDTO>
			{
				new TopSellingProductDTO { Name = "Apple Strudel", TotalSold = 10 },
				new TopSellingProductDTO { Name = "Cherry Strudel", TotalSold = 5 }
			};
			_productServiceMock.Setup(s => s.GetTopSellingProductsAsync()).ReturnsAsync(topProducts);

			// Act
			var result = await _controller.GetTopSellingProducts();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<TopSellingProductDTO>>(okResult.Value);
			returnedList.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetProductOverview_ReturnsOk()
		{
			// Arrange
			var overview = new List<ProductOverviewDTO>
			{
				new ProductOverviewDTO { ProductId = 1, Name = "Apple Strudel" },
				new ProductOverviewDTO { ProductId = 2, Name = "Cherry Strudel" }
			};
			_productServiceMock.Setup(s => s.GetProductOverviewAsync()).ReturnsAsync(overview);

			// Act
			var result = await _controller.GetProductOverview();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedOverview = Assert.IsType<List<ProductOverviewDTO>>(okResult.Value);
			returnedOverview.Should().HaveCount(2);
		}
	}
}
