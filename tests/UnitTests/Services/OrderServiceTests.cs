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
	public class OrderServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly OrderService _service;

		public OrderServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"OrderTestDb_{Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Seed initial data
			var user1 = new User
			{
				UserID = 2,
				Username = "john_doe",
				PasswordHash = "password123",
				Role = "User",
				FirstName = "John",
				LastName = "Doe",
				Email = "john@strudelshop.com",
				PhoneNumber = "0987654321",
				Address = "456 John St"
			};
			var user2 = new User
			{
				UserID = 3,
				Username = "jane_smith",
				PasswordHash = "password456",
				Role = "User",
				FirstName = "Jane",
				LastName = "Smith",
				Email = "jane@strudelshop.com",
				PhoneNumber = "1234567890",
				Address = "789 Jane Ave"
			};

			var orders = new List<Order>
			{
				new Order
				{
					OrderID = 1,
					UserID = 2,
					OrderDate = DateTime.Now.AddDays(-5),
					DeliveryDate = DateTime.Now.AddDays(-2),
					TotalAmount = 20.00m,
					PaymentStatus = "Pending",
					User = user1,
					OrderItems = new List<OrderItem>
					{
						new OrderItem { OrderItemID = 1, OrderID = 1, ProductID = 10, Quantity = 2, Price = 10.00m }
					}
				},
				new Order
				{
					OrderID = 2,
					UserID = 3,
					OrderDate = DateTime.Now.AddDays(-3),
					DeliveryDate = DateTime.Now.AddDays(-1),
					TotalAmount = 50.00m,
					PaymentStatus = "Paid",
					User = user2,
					OrderItems = new List<OrderItem>
					{
						new OrderItem { OrderItemID = 2, OrderID = 2, ProductID = 11, Quantity = 5, Price = 10.00m }
					}
				}
			};

			_dbContext.Users.AddRange(user1, user2);
			_dbContext.Orders.AddRange(orders);
			_dbContext.SaveChanges();

			_service = new OrderService(_dbContext);
		}

		/// <summary>
		/// Tests that GetOrderByIdAsync returns the order if it is found.
		/// </summary>
		[Fact]
		public async Task GetOrderByIdAsync_WhenFound_ReturnsOrder()
		{
			// Arrange
			var orderId = 1;

			// Act
			var result = await _service.GetOrderByIdAsync(orderId);

			// Assert
			result.Should().NotBeNull();
			result.OrderID.Should().Be(orderId);
			result.UserID.Should().Be(2);
			result.TotalAmount.Should().Be(20.00m);
			result.PaymentStatus.Should().Be("Pending");
			result.OrderItems.Should().HaveCount(1);
			result.User.FirstName.Should().Be("John");
		}

		/// <summary>
		/// Tests that GetOrderByIdAsync returns null if the order is not found.
		/// </summary>
		[Fact]
		public async Task GetOrderByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			var orderId = 999;

			// Act
			var result = await _service.GetOrderByIdAsync(orderId);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllOrdersAsync returns the expected list of orders.
		/// </summary>
		[Fact]
		public async Task GetAllOrdersAsync_ReturnsOrders()
		{
			// Act
			var result = await _service.GetAllOrdersAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(o => o.OrderID == 1);
			result.Should().Contain(o => o.OrderID == 2);
		}

		/// <summary>
		/// Tests that CreateOrderAsync adds and saves a new order.
		/// </summary>
		[Fact]
		public async Task CreateOrderAsync_AddsAndSaves()
		{
			// Arrange
			var newUser = new User
			{
				UserID = 4,
				Username = "alice_brown",
				PasswordHash = "password789",
				Role = "User",
				FirstName = "Alice",
				LastName = "Brown",
				Email = "alice@strudelshop.com",
				PhoneNumber = "5555555555",
				Address = "101 Alice Blvd"
			};
			_dbContext.Users.Add(newUser);
			await _dbContext.SaveChangesAsync();

			var newOrder = new Order
			{
				OrderID = 10,
				UserID = 4,
				OrderDate = DateTime.Now,
				DeliveryDate = DateTime.Now.AddDays(3),
				TotalAmount = 30.00m,
				PaymentStatus = "Paid",
				OrderItems = new List<OrderItem>
				{
					new OrderItem { OrderItemID = 3, OrderID = 10, ProductID = 12, Quantity = 3, Price = 10.00m }
				}
			};

			// Act
			await _service.CreateOrderAsync(newOrder);

			// Assert
			var createdOrder = await _dbContext.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.OrderID == 10);
			createdOrder.Should().NotBeNull();
			createdOrder.UserID.Should().Be(4);
			createdOrder.TotalAmount.Should().Be(30.00m);
			createdOrder.PaymentStatus.Should().Be("Paid");
			createdOrder.OrderItems.Should().HaveCount(1);
			createdOrder.OrderItems.First().ProductID.Should().Be(12);
		}

		/// <summary>
		/// Tests that UpdateOrderAsync updates the order and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateOrderAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingOrder = await _dbContext.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.OrderID == 1);
			existingOrder.TotalAmount = 25.00m;
			existingOrder.PaymentStatus = "Shipped";
			existingOrder.OrderItems.First().Price = 12.50m;

			// Act
			await _service.UpdateOrderAsync(existingOrder);

			// Assert
			var updatedOrder = await _dbContext.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.OrderID == 1);
			updatedOrder.TotalAmount.Should().Be(25.00m);
			updatedOrder.PaymentStatus.Should().Be("Shipped");
			updatedOrder.OrderItems.First().Price.Should().Be(12.50m);
		}

		/// <summary>
		/// Tests that DeleteOrderAsync removes the order if found, then saves.
		/// </summary>
		[Fact]
		public async Task DeleteOrderAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingOrder = await _dbContext.Orders.FindAsync(2);

			// Act
			await _service.DeleteOrderAsync(2);

			// Assert
			var deletedOrder = await _dbContext.Orders.FindAsync(2);
			deletedOrder.Should().BeNull();
		}

		/// <summary>
		/// Tests that DeleteOrderAsync does nothing if the order is not found.
		/// </summary>
		[Fact]
		public async Task DeleteOrderAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			var nonExistentOrderId = 999;

			// Act
			await _service.DeleteOrderAsync(nonExistentOrderId);

			// Assert
			var order = await _dbContext.Orders.FindAsync(nonExistentOrderId);
			order.Should().BeNull();
			_dbContext.Orders.Count().Should().Be(2); // Ensure no orders were removed
		}

		/// <summary>
		/// Tests that GetCustomerOrderSummariesAsync returns a list of CustomerOrderSummaryDTO.
		/// </summary>
		[Fact]
		public async Task GetCustomerOrderSummariesAsync_ReturnsSummaries()
		{
			// Act
			var summaries = await _service.GetCustomerOrderSummariesAsync();

			// Assert
			summaries.Should().HaveCount(2);
			summaries.Should().Contain(s => s.OrderId == 1 && s.CustomerName == "John Doe" && s.PaymentStatus == "Pending");
			summaries.Should().Contain(s => s.OrderId == 2 && s.CustomerName == "Jane Smith" && s.PaymentStatus == "Paid");
		}

		/// <summary>
		/// Tests that GetOrderHistoryAsync returns the orders belonging to a specified user.
		/// </summary>
		[Fact]
		public async Task GetOrderHistoryAsync_ReturnsMatchingOrders()
		{
			// Arrange
			var userId = 2;

			// Act
			var result = await _service.GetOrderHistoryAsync(userId);

			// Assert
			result.Should().HaveCount(1);
			result.Should().Contain(o => o.OrderId == 1 && o.TotalAmount == 20.00m && o.PaymentStatus == "Pending");
		}

		/// <summary>
		/// Tests that GetOrderDetailsAsync returns an OrderDetailsDTO when the order is found.
		/// </summary>
		[Fact]
		public async Task GetOrderDetailsAsync_WhenOrderFound_ReturnsDetails()
		{
			// Arrange
			var orderId = 1;

			// Act
			var result = await _service.GetOrderDetailsAsync(orderId);

			// Assert
			result.Should().NotBeNull();
			result.OrderId.Should().Be(orderId);
			result.CustomerFirstName.Should().Be("John");
			result.CustomerLastName.Should().Be("Doe");
			result.CustomerEmail.Should().Be("john@strudelshop.com");
			result.CustomerPhoneNumber.Should().Be("0987654321");
			result.CustomerAddress.Should().Be("456 John St");
			result.OrderItems.Should().HaveCount(1);
			result.OrderTotalAmount.Should().Be(20.00m);
			result.PaymentStatus.Should().Be("Pending");
		}

		/// <summary>
		/// Tests that GetOrderDetailsAsync returns null when the specified order is not found.
		/// </summary>
		[Fact]
		public async Task GetOrderDetailsAsync_WhenOrderNotFound_ReturnsNull()
		{
			// Arrange
			var orderId = 999;

			// Act
			var result = await _service.GetOrderDetailsAsync(orderId);

			// Assert
			result.Should().BeNull();
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
