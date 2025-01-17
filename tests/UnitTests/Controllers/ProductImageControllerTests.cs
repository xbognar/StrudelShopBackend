using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using StrudelShop.DataAccess.Services.Interfaces;
using WebAPI.Controllers;

namespace UnitTests.Controllers
{
	public class ProductImageControllerTests
	{
		private readonly Mock<IProductImageService> _productImageServiceMock;
		private readonly ProductImageController _controller;

		public ProductImageControllerTests()
		{
			_productImageServiceMock = new Mock<IProductImageService>();
			_controller = new ProductImageController(_productImageServiceMock.Object);
		}

		[Fact]
		public async Task GetAllProductImages_ReturnsOkWithList()
		{
			// Arrange
			var images = new List<ProductImage>
			{
				new ProductImage { ImageID = 1, ProductID = 10 },
				new ProductImage { ImageID = 2, ProductID = 20 }
			};
			_productImageServiceMock.Setup(s => s.GetAllProductImagesAsync()).ReturnsAsync(images);

			// Act
			var result = await _controller.GetAllProductImages();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedImages = Assert.IsType<List<ProductImage>>(okResult.Value);
			returnedImages.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetProductImageById_WhenFound_ReturnsOk()
		{
			// Arrange
			var image = new ProductImage { ImageID = 5, ProductID = 100 };
			_productImageServiceMock.Setup(s => s.GetProductImageByIdAsync(5)).ReturnsAsync(image);

			// Act
			var result = await _controller.GetProductImageById(5);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedImage = Assert.IsType<ProductImage>(okResult.Value);
			returnedImage.ImageID.Should().Be(5);
		}

		[Fact]
		public async Task GetProductImageById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_productImageServiceMock.Setup(s => s.GetProductImageByIdAsync(999)).ReturnsAsync((ProductImage)null);

			// Act
			var result = await _controller.GetProductImageById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateProductImage_ReturnsCreatedAtAction()
		{
			// Arrange
			var newImage = new ProductImage { ImageID = 7, ProductID = 200 };
			_productImageServiceMock.Setup(s => s.CreateProductImageAsync(newImage)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateProductImage(newImage);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(7);
			createdResult.Value.Should().Be(newImage);
		}

		[Fact]
		public async Task UpdateProductImage_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var existingImage = new ProductImage { ImageID = 10, ProductID = 300 };
			_productImageServiceMock.Setup(s => s.UpdateProductImageAsync(existingImage)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateProductImage(10, existingImage);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateProductImage_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var image = new ProductImage { ImageID = 10 };

			// Act
			var result = await _controller.UpdateProductImage(999, image);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task DeleteProductImage_ReturnsNoContent()
		{
			// Arrange
			_productImageServiceMock.Setup(s => s.DeleteProductImageAsync(2)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteProductImage(2);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
