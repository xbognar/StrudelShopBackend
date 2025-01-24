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
using DataAccess.DTOs;

namespace UnitTests.Services
{
	public class ProductServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ProductService _service;

		public ProductServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"ProductTestDb_{Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Seed initial data with all required properties
			var product1 = new Product
			{
				ProductID = 1,
				Name = "Apple Strudel",
				Description = "Delicious apple-filled pastry.",
				Price = 10.99m,
				ImageURL = "apple_main.jpg"
			};
			var product2 = new Product
			{
				ProductID = 2,
				Name = "Cherry Strudel",
				Description = "Tasty cherry-filled pastry.",
				Price = 12.99m,
				ImageURL = "cherry_main.jpg"
			};

			_dbContext.Products.AddRange(product1, product2);

			var productImages = new List<ProductImage>
			{
				new ProductImage { ImageID = 1, ProductID = 1, ImageURL = "apple1.jpg" },
				new ProductImage { ImageID = 2, ProductID = 1, ImageURL = "apple2.jpg" },
				new ProductImage { ImageID = 3, ProductID = 2, ImageURL = "cherry1.jpg" }
			};

			_dbContext.ProductImages.AddRange(productImages);

			var orderItems = new List<OrderItem>
			{
				new OrderItem { OrderItemID = 1, OrderID = 1000, ProductID = 1, Quantity = 2, Price = 10.00m },
				new OrderItem { OrderItemID = 2, OrderID = 1001, ProductID = 2, Quantity = 5, Price = 12.00m }
			};

			_dbContext.OrderItems.AddRange(orderItems);
			_dbContext.SaveChanges();

			_service = new ProductService(_dbContext);
		}

		/// <summary>
		/// Tests that GetProductByIdAsync returns the product if it is found.
		/// </summary>
		[Fact]
		public async Task GetProductByIdAsync_WhenFound_ReturnsProduct()
		{
			// Arrange
			var productId = 1;

			// Act
			var result = await _service.GetProductByIdAsync(productId);

			// Assert
			result.Should().NotBeNull();
			result.ProductID.Should().Be(productId);
			result.Name.Should().Be("Apple Strudel");
			result.Price.Should().Be(10.99m);
			result.ImageURL.Should().Be("apple_main.jpg");
			result.ProductImages.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that GetProductByIdAsync returns null if the product is not found.
		/// </summary>
		[Fact]
		public async Task GetProductByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			var productId = 999;

			// Act
			var result = await _service.GetProductByIdAsync(productId);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllProductsAsync returns the expected list of products.
		/// </summary>
		[Fact]
		public async Task GetAllProductsAsync_ReturnsProducts()
		{
			// Act
			var result = await _service.GetAllProductsAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(p => p.ProductID == 1);
			result.Should().Contain(p => p.ProductID == 2);
		}

		/// <summary>
		/// Tests that CreateProductAsync adds and saves a new product.
		/// </summary>
		[Fact]
		public async Task CreateProductAsync_AddsAndSaves()
		{
			// Arrange
			var newProduct = new Product
			{
				ProductID = 10,
				Name = "Blueberry Strudel",
				Description = "Sweet blueberry-filled pastry.",
				Price = 14.99m,
				ImageURL = "blueberry_main.jpg"
			};

			// Act
			await _service.CreateProductAsync(newProduct);

			// Assert
			var createdProduct = await _dbContext.Products.FindAsync(10);
			createdProduct.Should().NotBeNull();
			createdProduct.Name.Should().Be("Blueberry Strudel");
			createdProduct.Description.Should().Be("Sweet blueberry-filled pastry.");
			createdProduct.Price.Should().Be(14.99m);
			createdProduct.ImageURL.Should().Be("blueberry_main.jpg");
		}

		/// <summary>
		/// Tests that UpdateProductAsync updates the product and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateProductAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingProduct = await _dbContext.Products.FindAsync(1);
			existingProduct.Name = "Green Apple Strudel";
			existingProduct.Price = 11.99m;
			existingProduct.ImageURL = "green_apple_main.jpg";
			existingProduct.Description = "Fresh green apple-filled pastry.";

			// Act
			await _service.UpdateProductAsync(existingProduct);

			// Assert
			var updatedProduct = await _dbContext.Products.FindAsync(1);
			updatedProduct.Name.Should().Be("Green Apple Strudel");
			updatedProduct.Price.Should().Be(11.99m);
			updatedProduct.ImageURL.Should().Be("green_apple_main.jpg");
			updatedProduct.Description.Should().Be("Fresh green apple-filled pastry.");
		}

		/// <summary>
		/// Tests that DeleteProductAsync removes the product if it is found, then saves.
		/// </summary>
		[Fact]
		public async Task DeleteProductAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingProduct = await _dbContext.Products.FindAsync(2);

			// Act
			await _service.DeleteProductAsync(2);

			// Assert
			var deletedProduct = await _dbContext.Products.FindAsync(2);
			deletedProduct.Should().BeNull();
		}

		/// <summary>
		/// Tests that DeleteProductAsync does nothing if the product is not found.
		/// </summary>
		[Fact]
		public async Task DeleteProductAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			var nonExistentProductId = 999;

			// Act
			await _service.DeleteProductAsync(nonExistentProductId);

			// Assert
			var product = await _dbContext.Products.FindAsync(nonExistentProductId);
			product.Should().BeNull();
			_dbContext.Products.Count().Should().Be(2);
		}

		/// <summary>
		/// Tests that GetTopSellingProductsAsync returns the correct groupings and totals.
		/// </summary>
		[Fact]
		public async Task GetTopSellingProductsAsync_ReturnsCorrectGroupings()
		{
			// Act
			var result = await _service.GetTopSellingProductsAsync();

			// Assert
			result.Should().HaveCount(2);

			var apple = result.First(r => r.Name == "Apple Strudel");
			apple.TotalSold.Should().Be(2);

			var cherry = result.First(r => r.Name == "Cherry Strudel");
			cherry.TotalSold.Should().Be(5);
		}

		/// <summary>
		/// Tests that GetProductOverviewAsync returns an overview including stock counts.
		/// </summary>
		[Fact]
		public async Task GetProductOverviewAsync_ReturnsOverviewData()
		{
			// Act
			var result = await _service.GetProductOverviewAsync();

			// Assert
			result.Should().HaveCount(2);

			var appleOverview = result.FirstOrDefault(o => o.ProductId == 1);
			appleOverview.Should().NotBeNull();
			appleOverview.Name.Should().Be("Apple Strudel");
			appleOverview.MainImageURL.Should().Be("apple_main.jpg");
			appleOverview.StockQuantity.Should().Be(2);

			var cherryOverview = result.FirstOrDefault(o => o.ProductId == 2);
			cherryOverview.Should().NotBeNull();
			cherryOverview.Name.Should().Be("Cherry Strudel");
			cherryOverview.MainImageURL.Should().Be("cherry_main.jpg");
			cherryOverview.StockQuantity.Should().Be(1);
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
