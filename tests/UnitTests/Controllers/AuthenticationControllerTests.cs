using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DataAccess.DTOs;
using DataAccess.Models;
using DataAccess.Services.Interfaces;
using WebAPI.Controllers;

namespace UnitTests.Controllers
{
	public class AuthenticationControllerTests
	{
		private readonly Mock<IAuthenticationService> _authServiceMock;
		private readonly AuthenticationController _controller;

		public AuthenticationControllerTests()
		{
			_authServiceMock = new Mock<IAuthenticationService>();
			_controller = new AuthenticationController(_authServiceMock.Object);
		}

		/// <summary>
		/// Tests that a successful registration returns OK with the success message.
		/// </summary>
		[Fact]
		public async Task Register_WhenRegistrationSucceeds_ReturnsOk()
		{
			// Arrange
			var newUser = new User { Username = "testUser" };
			_authServiceMock
				.Setup(s => s.RegisterUserAsync(newUser))
				.ReturnsAsync(true);

			// Act
			var result = await _controller.Register(newUser);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			actionResult.Value.Should().Be("Registration successful");
		}

		/// <summary>
		/// Tests that a failed registration returns BadRequest with the fail message.
		/// </summary>
		[Fact]
		public async Task Register_WhenRegistrationFails_ReturnsBadRequest()
		{
			// Arrange
			var newUser = new User { Username = "testUser" };
			_authServiceMock
				.Setup(s => s.RegisterUserAsync(newUser))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.Register(newUser);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			actionResult.Value.Should().Be("Registration failed.");
		}

		/// <summary>
		/// Tests that valid credentials during login return OK with a LoginResponseDTO.
		/// </summary>
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkWithLoginResponse()
		{
			// Arrange
			var loginRequest = new LoginRequestDTO { Username = "testUser", Password = "testPass" };
			var loginResponse = new LoginResponseDTO
			{
				UserId = 1,
				Token = "fake_jwt",
				Role = "User",
				FirstName = "Test",
				LastName = "User"
			};

			_authServiceMock
				.Setup(s => s.AuthenticateAsync(loginRequest.Username, loginRequest.Password))
				.ReturnsAsync(loginResponse);

			// Act
			var result = await _controller.Login(loginRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);
			returnedResponse.Should().BeEquivalentTo(loginResponse);
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized with the correct error message.
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var loginRequest = new LoginRequestDTO { Username = "testUser", Password = "wrongPass" };
			_authServiceMock
				.Setup(s => s.AuthenticateAsync(loginRequest.Username, loginRequest.Password))
				.ReturnsAsync((LoginResponseDTO)null);

			// Act
			var result = await _controller.Login(loginRequest);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
			unauthorizedResult.Value.Should().Be("Invalid credentials");
		}
	}
}
