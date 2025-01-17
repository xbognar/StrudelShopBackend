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
	public class UserControllerTests
	{
		private readonly Mock<IUserService> _userServiceMock;
		private readonly UserController _controller;

		public UserControllerTests()
		{
			_userServiceMock = new Mock<IUserService>();
			_controller = new UserController(_userServiceMock.Object);
		}

		[Fact]
		public async Task GetAllUsers_ReturnsOkWithList()
		{
			// Arrange
			var users = new List<User>
			{
				new User { UserID = 1, Username = "Alice" },
				new User { UserID = 2, Username = "Bob" }
			};
			_userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

			// Act
			var result = await _controller.GetAllUsers();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
			returnedUsers.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetUserById_WhenFound_ReturnsOk()
		{
			// Arrange
			var user = new User { UserID = 10, Username = "TestUser" };
			_userServiceMock.Setup(s => s.GetUserByIdAsync(10)).ReturnsAsync(user);

			// Act
			var result = await _controller.GetUserById(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUser = Assert.IsType<User>(okResult.Value);
			returnedUser.UserID.Should().Be(10);
		}

		[Fact]
		public async Task GetUserById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_userServiceMock.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User)null);

			// Act
			var result = await _controller.GetUserById(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateUser_ReturnsCreatedAtAction()
		{
			// Arrange
			var newUser = new User { UserID = 3, Username = "NewUser" };
			_userServiceMock.Setup(s => s.CreateUserAsync(newUser)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreateUser(newUser);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			createdResult.RouteValues["id"].Should().Be(3);
			createdResult.Value.Should().Be(newUser);
		}

		[Fact]
		public async Task UpdateUser_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var user = new User { UserID = 2, Username = "UpdateMe" };
			_userServiceMock.Setup(s => s.UpdateUserAsync(user)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateUser(2, user);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateUser_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var user = new User { UserID = 2 };

			// Act
			var result = await _controller.UpdateUser(999, user);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task DeleteUser_ReturnsNoContent()
		{
			// Arrange
			_userServiceMock.Setup(s => s.DeleteUserAsync(4)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteUser(4);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
