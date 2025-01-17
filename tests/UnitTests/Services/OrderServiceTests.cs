using System;
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
	public class OrderServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Order>> _orderDbSetMock;
		private readonly OrderService _service;

		public OrderServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_orderDbSetMock = new Mock<DbSet<Order>>();
			_dbContextMock.Setup(db => db.Orders).Returns(_orderDbSetMock.Object);

			_service = new OrderService(_dbContextMock.Object);
		}

		[Fact]
		public async Task GetOrderByIdAsync_WhenFound_ReturnsOrder()
		{
			// Arrange
			var order = new Order { OrderID = 1 };
			_dbContextMock
				.Setup(db => db.Orders
					.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.Product)
					.FirstOrDefaultAsync(o => o.OrderID == 1, default))
				.ReturnsAsync(order);

			// Act
			var result = await _service.GetOrderByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.OrderID.Should().Be(1);
		}

		[Fact]
		public async Task GetOrderByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock
				.Setup(db => db.Orders
					.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.Product)
					.FirstOrDefaultAsync(o => o.OrderID == 999, default))
				.ReturnsAsync((Order)null);

			// Act
			var result = await _service.GetOrderByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAllOrdersAsync_ReturnsOrders()
		{
			// Arrange
			var ordersData = new List<Order>
			{
				new Order { OrderID = 1 },
				new Order { OrderID = 2 },
			}.AsQueryable();

			_orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(ordersData.Provider);
			_orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(ordersData.Expression);
			_orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(ordersData.ElementType);
			_orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(ordersData.GetEnumerator());

			// Act
			var result = await _service.GetAllOrdersAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task CreateOrderAsync_AddsAndSaves()
		{
			// Arrange
			var newOrder = new Order { OrderID = 10 };

			// Act
			await _service.CreateOrderAsync(newOrder);

			// Assert
			_dbContextMock.Verify(db => db.Orders.AddAsync(newOrder, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task UpdateOrderAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingOrder = new Order { OrderID = 5 };

			// Act
			await _service.UpdateOrderAsync(existingOrder);

			// Assert
			_dbContextMock.Verify(db => db.Orders.Update(existingOrder), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteOrderAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingOrder = new Order { OrderID = 7 };
			_dbContextMock.Setup(db => db.Orders.FindAsync(7)).ReturnsAsync(existingOrder);

			// Act
			await _service.DeleteOrderAsync(7);

			// Assert
			_dbContextMock.Verify(db => db.Orders.Remove(existingOrder), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteOrderAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Orders.FindAsync(999)).ReturnsAsync((Order)null);

			// Act
			await _service.DeleteOrderAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.Orders.Remove(It.IsAny<Order>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		[Fact]
		public async Task GetOrderHistoryAsync_ReturnsMatchingOrders()
		{
			// Arrange
			var userId = 2;
			var mockOrders = new List<Order>
			{
				new Order { OrderID = 1, UserID = 2 },
				new Order { OrderID = 2, UserID = 2 },
				new Order { OrderID = 3, UserID = 3 },
			}.AsQueryable();

			var orderDbSetMock = new Mock<DbSet<Order>>();
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(mockOrders.Provider);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(mockOrders.Expression);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(mockOrders.ElementType);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(mockOrders.GetEnumerator());

			_dbContextMock.Setup(db => db.Orders).Returns(orderDbSetMock.Object);

			// Act
			var result = await _service.GetOrderHistoryAsync(userId);

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetOrderDetailsAsync_WhenOrderFound_ReturnsDetails()
		{
			// Arrange
			var order = new Order
			{
				OrderID = 10,
				TotalAmount = 25.50m,
				User = new User { FirstName = "Test", LastName = "User", Email = "test@test.com", PhoneNumber = "12345", Address = "SomeAddress" },
				OrderItems = new List<OrderItem>
				{
					new OrderItem { OrderItemID = 1, ProductID = 100, Quantity = 2 },
				}
			};

			_dbContextMock
				.Setup(db => db.Orders
					.Include(o => o.User)
					.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.Product)
					.FirstOrDefaultAsync(o => o.OrderID == 10, default))
				.ReturnsAsync(order);

			// Act
			var result = await _service.GetOrderDetailsAsync(10);

			// Assert
			result.Should().NotBeNull();
			result.OrderId.Should().Be(10);
			result.CustomerFirstName.Should().Be("Test");
			result.OrderItems.Should().HaveCount(1);
		}

		[Fact]
		public async Task GetOrderDetailsAsync_WhenOrderNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock
				.Setup(db => db.Orders
					.Include(o => o.User)
					.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.Product)
					.FirstOrDefaultAsync(o => o.OrderID == 999, default))
				.ReturnsAsync((Order)null);

			// Act
			var result = await _service.GetOrderDetailsAsync(999);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetCustomerOrderSummariesAsync_ReturnsSummaries()
		{
			// Arrange
			var mockOrders = new List<Order>
			{
				new Order
				{
					OrderID = 1,
					OrderDate = DateTime.Now,
					TotalAmount = 100,
					PaymentStatus = "Paid",
					User = new User { FirstName = "A", LastName = "B" }
				},
				new Order
				{
					OrderID = 2,
					OrderDate = DateTime.Now,
					TotalAmount = 200,
					PaymentStatus = "Pending",
					User = new User { FirstName = "C", LastName = "D" }
				}
			}.AsQueryable();

			var orderDbSetMock = new Mock<DbSet<Order>>();
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(mockOrders.Provider);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(mockOrders.Expression);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(mockOrders.ElementType);
			orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(mockOrders.GetEnumerator());

			_dbContextMock.Setup(db => db.Orders).Returns(orderDbSetMock.Object);

			// Act
			var summaries = await _service.GetCustomerOrderSummariesAsync();

			// Assert
			summaries.Should().HaveCount(2);
			summaries.First().OrderId.Should().Be(1);
			summaries.First().CustomerName.Should().Be("A B");
		}
	}
}
