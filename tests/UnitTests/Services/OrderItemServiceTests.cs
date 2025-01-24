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
	public class OrderItemServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly OrderItemService _service;

		public OrderItemServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"OrderItemTestDb_{System.Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Seed initial data
			_dbContext.OrderItems.AddRange(new List<OrderItem>
			{
				new OrderItem { OrderItemID = 1, OrderID = 1000, ProductID = 10, Quantity = 2, Price = 20.00m },
				new OrderItem { OrderItemID = 2, OrderID = 1001, ProductID = 11, Quantity = 1, Price = 10.00m }
			});
			_dbContext.SaveChanges();

			_service = new OrderItemService(_dbContext);
		}

		/// <summary>
		/// Tests that GetOrderItemByIdAsync returns the item if it is found.
		/// </summary>
		[Fact]
		public async Task GetOrderItemByIdAsync_WhenFound_ReturnsItem()
		{
			// Arrange
			var testItemId = 1;

			// Act
			var result = await _service.GetOrderItemByIdAsync(testItemId);

			// Assert
			result.Should().NotBeNull();
			result.OrderItemID.Should().Be(testItemId);
			result.OrderID.Should().Be(1000);
			result.ProductID.Should().Be(10);
			result.Quantity.Should().Be(2);
			result.Price.Should().Be(20.00m);
		}

		/// <summary>
		/// Tests that GetOrderItemByIdAsync returns null if the item does not exist.
		/// </summary>
		[Fact]
		public async Task GetOrderItemByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			var testItemId = 999;

			// Act
			var result = await _service.GetOrderItemByIdAsync(testItemId);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllOrderItemsAsync returns the full set of items.
		/// </summary>
		[Fact]
		public async Task GetAllOrderItemsAsync_ReturnsAll()
		{
			// Act
			var result = await _service.GetAllOrderItemsAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(o => o.OrderItemID == 1);
			result.Should().Contain(o => o.OrderItemID == 2);
		}

		/// <summary>
		/// Tests that CreateOrderItemAsync adds a new item and saves changes.
		/// </summary>
		[Fact]
		public async Task CreateOrderItemAsync_AddsAndSaves()
		{
			// Arrange
			var newItem = new OrderItem { OrderItemID = 10, OrderID = 1002, ProductID = 12, Quantity = 3, Price = 30.00m };

			// Act
			await _service.CreateOrderItemAsync(newItem);

			// Assert
			var createdItem = await _dbContext.OrderItems.FindAsync(10);
			createdItem.Should().NotBeNull();
			createdItem.OrderID.Should().Be(1002);
			createdItem.ProductID.Should().Be(12);
			createdItem.Quantity.Should().Be(3);
			createdItem.Price.Should().Be(30.00m);
		}

		/// <summary>
		/// Tests that UpdateOrderItemAsync updates the item and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateOrderItemAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingItem = await _dbContext.OrderItems.FindAsync(1);
			existingItem.Quantity = 5;
			existingItem.Price = 50.00m;

			// Act
			await _service.UpdateOrderItemAsync(existingItem);

			// Assert
			var updatedItem = await _dbContext.OrderItems.FindAsync(1);
			updatedItem.Quantity.Should().Be(5);
			updatedItem.Price.Should().Be(50.00m);
		}

		/// <summary>
		/// Tests that DeleteOrderItemAsync removes the item if found, then saves.
		/// </summary>
		[Fact]
		public async Task DeleteOrderItemAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingItem = await _dbContext.OrderItems.FindAsync(2);

			// Act
			await _service.DeleteOrderItemAsync(2);

			// Assert
			var deletedItem = await _dbContext.OrderItems.FindAsync(2);
			deletedItem.Should().BeNull();
		}

		/// <summary>
		/// Tests that DeleteOrderItemAsync does nothing if the item is not found.
		/// </summary>
		[Fact]
		public async Task DeleteOrderItemAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			var nonExistentItemId = 999;

			// Act
			await _service.DeleteOrderItemAsync(nonExistentItemId);

			// Assert
			var item = await _dbContext.OrderItems.FindAsync(nonExistentItemId);
			item.Should().BeNull();
			_dbContext.OrderItems.Count().Should().Be(2);
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
