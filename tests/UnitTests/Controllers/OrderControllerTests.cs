using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using DataAccess.Models;
using DataAccess.DTOs;
using WebAPI.Controllers;

namespace UnitTests.Controllers
{
	public class OrderControllerTests
	{
		private readonly Mock<IOrderService> _orderServiceMock;
		private readonly OrderController _controller;

		public OrderControllerTests()
		{
			_orderServiceMock = new Mock<IOrderService>();
			_controller = new OrderController(_orderServiceMock.Object);
		}

		[Fact]
		public async Task GetAllOrders_ReturnsOkWithOrders()
		{
			// Arrange
			var orders = new List<Order> { new Order { OrderID = 1 }, new Order { OrderID = 2 } };
			_orderServiceMock.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

			// Act
			var result = await _controller.GetAllOrders();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
			returnedOrders.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetOrderById_WhenFound_ReturnsOkWithOrder()
		{
			// Arrange
			var order = new Order { OrderID = 10 };
			_orderServiceMock.Setup(s => s.GetOrderByIdAsync(10)).ReturnsAsync(order);

			// Act
			var result = await _controller.GetOrderById(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedOrder = Assert.IsType<Order>(okResult.Value);
			returnedOrder.OrderID.Should().Be(10);
		}

		[Fact]
		public async Task GetOrderById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_orderServiceMock.Setup(s => s.GetOrderByIdAsync(999)).ReturnsAsync((Order)null);

			// Act
			var result = await _controller.GetOrderById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateOrder_ReturnsCreatedAtAction()
		{
			// Arrange
			var newOrder = new Order { OrderID = 5 };
			_orderServiceMock.Setup(s => s.CreateOrderAsync(newOrder)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateOrder(newOrder);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newOrder);
		}

		[Fact]
		public async Task UpdateOrder_WhenIdMatchesAndServiceSucceeds_ReturnsNoContent()
		{
			// Arrange
			var order = new Order { OrderID = 3 };
			_orderServiceMock.Setup(s => s.UpdateOrderAsync(order)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateOrder(3, order);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateOrder_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var order = new Order { OrderID = 2 };

			// Act
			var result = await _controller.UpdateOrder(999, order);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task DeleteOrder_ReturnsNoContent()
		{
			// Arrange
			_orderServiceMock.Setup(s => s.DeleteOrderAsync(5)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteOrder(5);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task GetOrderHistory_ReturnsOkWithHistory()
		{
			// Arrange
			var userId = 10;
			var history = new List<OrderHistoryDTO>
			{
				new OrderHistoryDTO { OrderId = 1 },
				new OrderHistoryDTO { OrderId = 2 }
			};

			_orderServiceMock.Setup(s => s.GetOrderHistoryAsync(userId)).ReturnsAsync(history);

			// Act
			var result = await _controller.GetOrderHistory(userId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedHistory = Assert.IsType<List<OrderHistoryDTO>>(okResult.Value);
			returnedHistory.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetOrderDetails_ReturnsOkWithDetails()
		{
			// Arrange
			var details = new OrderDetailsDTO { OrderId = 5 };
			_orderServiceMock.Setup(s => s.GetOrderDetailsAsync(5)).ReturnsAsync(details);

			// Act
			var result = await _controller.GetOrderDetails(5);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedDetails = Assert.IsType<OrderDetailsDTO>(okResult.Value);
			returnedDetails.OrderId.Should().Be(5);
		}

		[Fact]
		public async Task GetCustomerOrderSummaries_ReturnsOkWithSummaries()
		{
			// Arrange
			var summaries = new List<CustomerOrderSummaryDTO>
			{
				new CustomerOrderSummaryDTO { OrderId = 1 },
				new CustomerOrderSummaryDTO { OrderId = 2 }
			};
			_orderServiceMock.Setup(s => s.GetCustomerOrderSummariesAsync()).ReturnsAsync(summaries);

			// Act
			var result = await _controller.GetCustomerOrderSummaries();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedSummaries = Assert.IsType<List<CustomerOrderSummaryDTO>>(okResult.Value);
			returnedSummaries.Should().HaveCount(2);
		}
	}
}
