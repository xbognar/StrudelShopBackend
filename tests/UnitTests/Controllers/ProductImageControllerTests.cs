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

		/// <summary>
		/// Tests that GetAllProductImages returns OkObjectResult with a list of product images.
		/// </summary>
		[Fact]
		public async Task GetAllProductImages_ReturnsOkWithList()
		{
			// Arrange
			var images = new List<ProductImage>
			{
				new ProductImage { ImageID = 1, ProductID = 10, ImageURL = "image1.jpg" },
				new ProductImage { ImageID = 2, ProductID = 20, ImageURL = "image2.jpg" }
			};
			_productImageServiceMock.Setup(s => s.GetAllProductImagesAsync()).ReturnsAsync(images);

			// Act
			var result = await _controller.GetAllProductImages();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedImages = Assert.IsType<List<ProductImage>>(okResult.Value);
			returnedImages.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that GetProductImageById returns OkObjectResult when the image is found.
		/// </summary>
		[Fact]
		public async Task GetProductImageById_WhenFound_ReturnsOk()
		{
			// Arrange
			var image = new ProductImage { ImageID = 5, ProductID = 100, ImageURL = "image5.jpg" };
			_productImageServiceMock.Setup(s => s.GetProductImageByIdAsync(5)).ReturnsAsync(image);

			// Act
			var result = await _controller.GetProductImageById(5);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedImage = Assert.IsType<ProductImage>(okResult.Value);
			returnedImage.ImageID.Should().Be(5);
			returnedImage.ProductID.Should().Be(100);
			returnedImage.ImageURL.Should().Be("image5.jpg");
		}

		/// <summary>
		/// Tests that GetProductImageById returns NotFound when the image does not exist.
		/// </summary>
		[Fact]
		public async Task GetProductImageById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_productImageServiceMock.Setup(s => s.GetProductImageByIdAsync(999)).ReturnsAsync((ProductImage)null);

			// Act
			var result = await _controller.GetProductImageById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that CreateProductImage returns CreatedAtAction with the correct route values.
		/// </summary>
		[Fact]
		public async Task CreateProductImage_ReturnsCreatedAtAction()
		{
			// Arrange
			var newImage = new ProductImage { ImageID = 7, ProductID = 200, ImageURL = "image7.jpg" };
			_productImageServiceMock.Setup(s => s.CreateProductImageAsync(newImage)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateProductImage(newImage);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(7);
			createdResult.Value.Should().Be(newImage);
		}

		/// <summary>
		/// Tests that UpdateProductImage returns NoContent when the ID matches.
		/// </summary>
		[Fact]
		public async Task UpdateProductImage_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var existingImage = new ProductImage { ImageID = 10, ProductID = 300, ImageURL = "updated_image10.jpg" };
			_productImageServiceMock.Setup(s => s.UpdateProductImageAsync(existingImage)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateProductImage(10, existingImage);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that UpdateProductImage returns BadRequest when the route ID does not match the object’s ID.
		/// </summary>
		[Fact]
		public async Task UpdateProductImage_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var image = new ProductImage { ImageID = 10, ProductID = 300, ImageURL = "image10.jpg" };

			// Act
			var result = await _controller.UpdateProductImage(999, image);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		/// <summary>
		/// Tests that DeleteProductImage returns NoContent when deletion succeeds.
		/// </summary>
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
