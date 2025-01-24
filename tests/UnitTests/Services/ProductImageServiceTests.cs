using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DataAccess.Services;
using StrudelShop.DataAccess.DataAccess;
using DataAccess.Models;

namespace UnitTests.Services
{
	public class ProductImageServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ProductImageService _service;

		public ProductImageServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"ProductImageTestDb_{Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Seed initial data with all required properties
			var product1 = new Product
			{
				ProductID = 10,
				Name = "Apple Strudel",
				Description = "Delicious apple-filled pastry.",
				Price = 10.99m,
				ImageURL = "apple_main.jpg"
			};
			var product2 = new Product
			{
				ProductID = 15,
				Name = "Cherry Strudel",
				Description = "Tasty cherry-filled pastry.",
				Price = 12.99m,
				ImageURL = "cherry_main.jpg"
			};

			_dbContext.Products.AddRange(product1, product2);

			var productImages = new List<ProductImage>
			{
				new ProductImage { ImageID = 1, ProductID = 10, ImageURL = "apple1.jpg" },
				new ProductImage { ImageID = 2, ProductID = 10, ImageURL = "apple2.jpg" },
				new ProductImage { ImageID = 3, ProductID = 15, ImageURL = "cherry1.jpg" }
			};

			_dbContext.ProductImages.AddRange(productImages);
			_dbContext.SaveChanges();

			_service = new ProductImageService(_dbContext);
		}

		/// <summary>
		/// Tests that GetProductImageByIdAsync returns the image when found.
		/// </summary>
		[Fact]
		public async Task GetProductImageByIdAsync_WhenFound_ReturnsImage()
		{
			// Arrange
			var testImageId = 1;

			// Act
			var result = await _service.GetProductImageByIdAsync(testImageId);

			// Assert
			result.Should().NotBeNull();
			result.ImageID.Should().Be(testImageId);
			result.ProductID.Should().Be(10);
			result.ImageURL.Should().Be("apple1.jpg");
		}

		/// <summary>
		/// Tests that GetProductImageByIdAsync returns null when the image is not found.
		/// </summary>
		[Fact]
		public async Task GetProductImageByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			var testImageId = 999;

			// Act
			var result = await _service.GetProductImageByIdAsync(testImageId);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllProductImagesAsync returns all images in the DbSet.
		/// </summary>
		[Fact]
		public async Task GetAllProductImagesAsync_ReturnsAllImages()
		{
			// Act
			var result = await _service.GetAllProductImagesAsync();

			// Assert
			result.Should().HaveCount(3);
			result.Should().Contain(pi => pi.ImageID == 1);
			result.Should().Contain(pi => pi.ImageID == 2);
			result.Should().Contain(pi => pi.ImageID == 3);
		}

		/// <summary>
		/// Tests that CreateProductImageAsync adds and saves a new product image.
		/// </summary>
		[Fact]
		public async Task CreateProductImageAsync_AddsAndSaves()
		{
			// Arrange
			var newImage = new ProductImage { ImageID = 4, ProductID = 15, ImageURL = "cherry2.jpg" };

			// Act
			await _service.CreateProductImageAsync(newImage);

			// Assert
			var createdImage = await _dbContext.ProductImages.FindAsync(4);
			createdImage.Should().NotBeNull();
			createdImage.ProductID.Should().Be(15);
			createdImage.ImageURL.Should().Be("cherry2.jpg");
		}

		/// <summary>
		/// Tests that UpdateProductImageAsync updates the image and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateProductImageAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingImage = await _dbContext.ProductImages.FindAsync(1);
			existingImage.ImageURL = "updated_apple1.jpg";

			// Act
			await _service.UpdateProductImageAsync(existingImage);

			// Assert
			var updatedImage = await _dbContext.ProductImages.FindAsync(1);
			updatedImage.ImageURL.Should().Be("updated_apple1.jpg");
		}

		/// <summary>
		/// Tests that DeleteProductImageAsync removes and saves the image if found.
		/// </summary>
		[Fact]
		public async Task DeleteProductImageAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingImage = await _dbContext.ProductImages.FindAsync(2);

			// Act
			await _service.DeleteProductImageAsync(2);

			// Assert
			var deletedImage = await _dbContext.ProductImages.FindAsync(2);
			deletedImage.Should().BeNull();
		}

		/// <summary>
		/// Tests that DeleteProductImageAsync does nothing if the image is not found.
		/// </summary>
		[Fact]
		public async Task DeleteProductImageAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			var nonExistentImageId = 999;

			// Act
			await _service.DeleteProductImageAsync(nonExistentImageId);

			// Assert
			var image = await _dbContext.ProductImages.FindAsync(nonExistentImageId);
			image.Should().BeNull();
			_dbContext.ProductImages.Count().Should().Be(3);
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
