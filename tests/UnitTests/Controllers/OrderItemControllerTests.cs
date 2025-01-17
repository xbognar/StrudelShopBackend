using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using DataAccess.Models;
using WebAPI.Controllers;

namespace UnitTests.Controllers
{
	public class OrderItemControllerTests
	{
		private readonly Mock<IOrderItemService> _orderItemServiceMock;
		private readonly OrderItemController _controller;

		public OrderItemControllerTests()
		{
			_orderItemServiceMock = new Mock<IOrderItemService>();
			_controller = new OrderItemController(_orderItemServiceMock.Object);
		}

		[Fact]
		public async Task GetAllOrderItems_ReturnsOkWithList()
		{
			// Arrange
			var items = new List<OrderItem> { new OrderItem { OrderItemID = 1 }, new OrderItem { OrderItemID = 2 } };
			_orderItemServiceMock.Setup(s => s.GetAllOrderItemsAsync()).ReturnsAsync(items);

			// Act
			var result = await _controller.GetAllOrderItems();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedItems = Assert.IsType<List<OrderItem>>(okResult.Value);
			returnedItems.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetOrderItemById_WhenFound_ReturnsOk()
		{
			// Arrange
			var orderItem = new OrderItem { OrderItemID = 10 };
			_orderItemServiceMock.Setup(s => s.GetOrderItemByIdAsync(10)).ReturnsAsync(orderItem);

			// Act
			var result = await _controller.GetOrderItemById(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedItem = Assert.IsType<OrderItem>(okResult.Value);
			returnedItem.OrderItemID.Should().Be(10);
		}

		[Fact]
		public async Task GetOrderItemById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_orderItemServiceMock.Setup(s => s.GetOrderItemByIdAsync(999)).ReturnsAsync((OrderItem)null);

			// Act
			var result = await _controller.GetOrderItemById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateOrderItem_ReturnsCreatedAtAction()
		{
			// Arrange
			var newItem = new OrderItem { OrderItemID = 5 };
			_orderItemServiceMock.Setup(s => s.CreateOrderItemAsync(newItem)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateOrderItem(newItem);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newItem);
		}

		[Fact]
		public async Task UpdateOrderItem_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var item = new OrderItem { OrderItemID = 2 };
			_orderItemServiceMock.Setup(s => s.UpdateOrderItemAsync(item)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateOrderItem(2, item);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateOrderItem_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var item = new OrderItem { OrderItemID = 2 };

			// Act
			var result = await _controller.UpdateOrderItem(999, item);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task DeleteOrderItem_ReturnsNoContent()
		{
			// Arrange
			_orderItemServiceMock.Setup(s => s.DeleteOrderItemAsync(3)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteOrderItem(3);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
