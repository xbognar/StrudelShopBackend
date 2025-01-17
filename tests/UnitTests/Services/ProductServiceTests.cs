using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using DataAccess.Services;
using StrudelShop.DataAccess.DataAccess;
using DataAccess.Models;
using DataAccess.DTOs;

namespace UnitTests.Services
{
	public class ProductServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Product>> _productDbSetMock;
		private readonly ProductService _service;

		public ProductServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_productDbSetMock = new Mock<DbSet<Product>>();
			_dbContextMock.Setup(db => db.Products).Returns(_productDbSetMock.Object);

			_service = new ProductService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests that GetProductByIdAsync returns the product if it is found.
		/// </summary>
		[Fact]
		public async Task GetProductByIdAsync_WhenFound_ReturnsProduct()
		{
			// Arrange
			var testProduct = new Product { ProductID = 1, Name = "Apple Strudel" };
			_dbContextMock
				.Setup(db => db.Products
					.Include(p => p.ProductImages)
					.FirstOrDefaultAsync(p => p.ProductID == 1, default))
				.ReturnsAsync(testProduct);

			// Act
			var result = await _service.GetProductByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.ProductID.Should().Be(1);
			result.Name.Should().Be("Apple Strudel");
		}

		/// <summary>
		/// Tests that GetProductByIdAsync returns null if the product is not found.
		/// </summary>
		[Fact]
		public async Task GetProductByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock
				.Setup(db => db.Products
					.Include(p => p.ProductImages)
					.FirstOrDefaultAsync(p => p.ProductID == 999, default))
				.ReturnsAsync((Product)null);

			// Act
			var result = await _service.GetProductByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllProductsAsync returns the expected list of products.
		/// </summary>
		[Fact]
		public async Task GetAllProductsAsync_ReturnsProducts()
		{
			// Arrange
			var productsData = new List<Product>
			{
				new Product { ProductID = 1, Name = "Apple Strudel" },
				new Product { ProductID = 2, Name = "Cherry Strudel" }
			}.AsQueryable();

			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productsData.Provider);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productsData.Expression);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productsData.ElementType);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productsData.GetEnumerator());

			// Act
			var result = await _service.GetAllProductsAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that CreateProductAsync adds and saves a new product.
		/// </summary>
		[Fact]
		public async Task CreateProductAsync_AddsAndSaves()
		{
			// Arrange
			var newProduct = new Product { ProductID = 10, Name = "New Strudel" };

			// Act
			await _service.CreateProductAsync(newProduct);

			// Assert
			_dbContextMock.Verify(db => db.Products.AddAsync(newProduct, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that UpdateProductAsync updates the product and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateProductAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingProduct = new Product { ProductID = 5, Name = "UpdateMe" };

			// Act
			await _service.UpdateProductAsync(existingProduct);

			// Assert
			_dbContextMock.Verify(db => db.Products.Update(existingProduct), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteProductAsync removes the product if it is found, then saves.
		/// </summary>
		[Fact]
		public async Task DeleteProductAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingProduct = new Product { ProductID = 7, Name = "ToDelete" };
			_dbContextMock.Setup(db => db.Products.FindAsync(7)).ReturnsAsync(existingProduct);

			// Act
			await _service.DeleteProductAsync(7);

			// Assert
			_dbContextMock.Verify(db => db.Products.Remove(existingProduct), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteProductAsync does nothing if the product is not found.
		/// </summary>
		[Fact]
		public async Task DeleteProductAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Products.FindAsync(999)).ReturnsAsync((Product)null);

			// Act
			await _service.DeleteProductAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.Products.Remove(It.IsAny<Product>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		/// <summary>
		/// Tests that GetTopSellingProductsAsync returns the correct groupings and totals.
		/// </summary>
		[Fact]
		public async Task GetTopSellingProductsAsync_ReturnsCorrectGroupings()
		{
			// Arrange
			var mockOrderItems = new List<OrderItem>
			{
				new OrderItem { OrderID = 1, ProductID = 1, Quantity = 2 },
				new OrderItem { OrderID = 2, ProductID = 2, Quantity = 3 },
				new OrderItem { OrderID = 3, ProductID = 1, Quantity = 4 }
			}.AsQueryable();

			var mockOrderItemDbSet = new Mock<DbSet<OrderItem>>();
			mockOrderItemDbSet.As<IQueryable<OrderItem>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
			mockOrderItemDbSet.As<IQueryable<OrderItem>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
			mockOrderItemDbSet.As<IQueryable<OrderItem>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
			mockOrderItemDbSet.As<IQueryable<OrderItem>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());
			_dbContextMock.Setup(db => db.OrderItems).Returns(mockOrderItemDbSet.Object);

			var mockProducts = new List<Product>
			{
				new Product { ProductID = 1, Name = "Apple Strudel" },
				new Product { ProductID = 2, Name = "Cherry Strudel" }
			}.AsQueryable();

			var mockProductDbSet = new Mock<DbSet<Product>>();
			mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(mockProducts.Provider);
			mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(mockProducts.Expression);
			mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(mockProducts.ElementType);
			mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(mockProducts.GetEnumerator());
			_dbContextMock.Setup(db => db.Products).Returns(mockProductDbSet.Object);

			// Act
			var result = await _service.GetTopSellingProductsAsync();

			// Assert
			result.Should().HaveCount(2);

			var apple = result.First(r => r.Name == "Apple Strudel");
			apple.TotalSold.Should().Be(6); // 2 + 4
			var cherry = result.First(r => r.Name == "Cherry Strudel");
			cherry.TotalSold.Should().Be(3);
		}

		/// <summary>
		/// Tests that GetProductOverviewAsync returns an overview including stock counts.
		/// </summary>
		[Fact]
		public async Task GetProductOverviewAsync_ReturnsOverviewData()
		{
			// Arrange
			var testProducts = new List<Product>
			{
				new Product
				{
					ProductID = 1,
					Name = "Apple Strudel",
					Price = 10.99m,
					ImageURL = "apple.jpg",
					ProductImages = new List<ProductImage> { new ProductImage(), new ProductImage() }
				},
				new Product
				{
					ProductID = 2,
					Name = "Cherry Strudel",
					Price = 12.99m,
					ImageURL = "cherry.jpg",
					ProductImages = new List<ProductImage>()
				}
			}.AsQueryable();

			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(testProducts.Provider);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(testProducts.Expression);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(testProducts.ElementType);
			_productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(testProducts.GetEnumerator());

			// Act
			var result = await _service.GetProductOverviewAsync();

			// Assert
			result.Should().HaveCount(2);

			var first = result.First(r => r.ProductId == 1);
			first.MainImageURL.Should().Be("apple.jpg");
			first.StockQuantity.Should().Be(2);
		}
	}
}
