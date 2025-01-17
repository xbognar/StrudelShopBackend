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

namespace UnitTests.Services
{
	public class OrderItemServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<OrderItem>> _orderItemDbSetMock;
		private readonly OrderItemService _service;

		public OrderItemServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_orderItemDbSetMock = new Mock<DbSet<OrderItem>>();
			_dbContextMock.Setup(db => db.OrderItems).Returns(_orderItemDbSetMock.Object);

			_service = new OrderItemService(_dbContextMock.Object);
		}

		[Fact]
		public async Task GetOrderItemByIdAsync_WhenFound_ReturnsItem()
		{
			// Arrange
			var testItem = new OrderItem { OrderItemID = 1 };
			_dbContextMock.Setup(db => db.OrderItems.FindAsync(1)).ReturnsAsync(testItem);

			// Act
			var result = await _service.GetOrderItemByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.OrderItemID.Should().Be(1);
		}

		[Fact]
		public async Task GetOrderItemByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock.Setup(db => db.OrderItems.FindAsync(999)).ReturnsAsync((OrderItem)null);

			// Act
			var result = await _service.GetOrderItemByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAllOrderItemsAsync_ReturnsAll()
		{
			// Arrange
			var itemsData = new List<OrderItem>
			{
				new OrderItem { OrderItemID = 1 },
				new OrderItem { OrderItemID = 2 }
			}.AsQueryable();

			_orderItemDbSetMock.As<IQueryable<OrderItem>>().Setup(m => m.Provider).Returns(itemsData.Provider);
			_orderItemDbSetMock.As<IQueryable<OrderItem>>().Setup(m => m.Expression).Returns(itemsData.Expression);
			_orderItemDbSetMock.As<IQueryable<OrderItem>>().Setup(m => m.ElementType).Returns(itemsData.ElementType);
			_orderItemDbSetMock.As<IQueryable<OrderItem>>().Setup(m => m.GetEnumerator()).Returns(itemsData.GetEnumerator());

			// Act
			var result = await _service.GetAllOrderItemsAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task CreateOrderItemAsync_AddsAndSaves()
		{
			// Arrange
			var newItem = new OrderItem { OrderItemID = 10 };

			// Act
			await _service.CreateOrderItemAsync(newItem);

			// Assert
			_dbContextMock.Verify(db => db.OrderItems.AddAsync(newItem, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task UpdateOrderItemAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingItem = new OrderItem { OrderItemID = 5 };

			// Act
			await _service.UpdateOrderItemAsync(existingItem);

			// Assert
			_dbContextMock.Verify(db => db.OrderItems.Update(existingItem), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteOrderItemAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingItem = new OrderItem { OrderItemID = 7 };
			_dbContextMock.Setup(db => db.OrderItems.FindAsync(7)).ReturnsAsync(existingItem);

			// Act
			await _service.DeleteOrderItemAsync(7);

			// Assert
			_dbContextMock.Verify(db => db.OrderItems.Remove(existingItem), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteOrderItemAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.OrderItems.FindAsync(999)).ReturnsAsync((OrderItem)null);

			// Act
			await _service.DeleteOrderItemAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.OrderItems.Remove(It.IsAny<OrderItem>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}
	}
}
