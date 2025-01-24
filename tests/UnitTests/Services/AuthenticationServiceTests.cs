using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataAccess.Services;
using DataAccess.Models;
using DataAccess.DTOs;
using StrudelShop.DataAccess.DataAccess;
using Moq;

namespace UnitTests.Services
{
	public class AuthenticationServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly AuthenticationService _authService;
		private readonly IConfiguration _config;

		public AuthenticationServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"AuthTestDb_{Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Initialize Configuration with a valid JWT key
			var inMemorySettings = new Dictionary<string, string> {
				{"ADMIN_USERNAME", "admin"},
				{"ADMIN_PASSWORD", "adminPass"},
				{"JWT_KEY", "ThisIsASuperSecureJWTKey1234567890!!"},
				{"JWT_TOKEN_EXPIRY_MINUTES", "30"},
				{"JWT_ISSUER", "TestIssuer"},
				{"JWT_AUDIENCE", "TestAudience"}
			};

			_config = new ConfigurationBuilder()
				.AddInMemoryCollection(inMemorySettings)
				.Build();

			// Seed Admin User
			var adminUser = new User
			{
				Username = _config["ADMIN_USERNAME"],
				Role = "Admin",
				Email = "admin@strudelshop.com",
				FirstName = "System",
				LastName = "Administrator",
				PhoneNumber = "123456",
				Address = "Admin Lane",
				PasswordHash = _config["ADMIN_PASSWORD"] 
			};
			_dbContext.Users.Add(adminUser);
			_dbContext.SaveChanges();

			_authService = new AuthenticationService(_dbContext, _config);
		}

		/// <summary>
		/// Tests that authenticating with admin credentials returns an admin LoginResponseDTO.
		/// </summary>
		[Fact]
		public async Task AuthenticateAsync_AdminCredentials_ReturnsAdminLoginResponse()
		{
			// Arrange
			var adminUsername = _config["ADMIN_USERNAME"];
			var adminPassword = _config["ADMIN_PASSWORD"];

			// Act
			var result = await _authService.AuthenticateAsync(adminUsername, adminPassword);

			// Assert
			result.Should().NotBeNull();
			result.Role.Should().Be("Admin");
			result.Token.Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests that authenticating with valid user credentials returns a user LoginResponseDTO.
		/// </summary>
		[Fact]
		public async Task AuthenticateAsync_ValidUserCredentials_ReturnsUserLoginResponse()
		{
			// Arrange
			var username = "testUser";
			var password = "testPass";

			var user = new User
			{
				Username = username,
				Role = "User",
				Email = "testuser@strudelshop.com",
				FirstName = "Test",
				LastName = "User",
				PhoneNumber = "654321",
				Address = "User Street 42",
				PasswordHash = password
			};

			_dbContext.Users.Add(user);
			_dbContext.SaveChanges();

			// Act
			var result = await _authService.AuthenticateAsync(username, password);

			// Assert
			result.Should().NotBeNull();
			result.Role.Should().Be("User");
			result.Token.Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests that authenticating with invalid credentials returns null.
		/// </summary>
		[Fact]
		public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
		{
			// Arrange
			var username = "nonExistentUser";
			var password = "wrongPass";

			// Act
			var result = await _authService.AuthenticateAsync(username, password);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that registering a new user successfully saves and returns true.
		/// </summary>
		[Fact]
		public async Task RegisterUserAsync_SuccessfulSave_ReturnsTrue()
		{
			// Arrange
			var newUser = new User
			{
				Username = "NewUser",
				Email = "newuser@strudelshop.com",
				FirstName = "New",
				LastName = "User",
				PhoneNumber = "789012",
				Address = "New Street 100",
				PasswordHash = "plainText"
			};

			// Act
			var result = await _authService.RegisterUserAsync(newUser);

			// Assert
			result.Should().BeTrue();
			var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "NewUser");
			createdUser.Should().NotBeNull();
			createdUser.Role.Should().Be("User");
			createdUser.PasswordHash.Should().Be("plainText");
		}

		/// <summary>
		/// Tests that if SaveChangesAsync returns 0, the registration fails (returns false).
		/// </summary>
		[Fact]
		public async Task RegisterUserAsync_NoChangesSaved_ReturnsFalse()
		{
			// Arrange
			var newUser = new User { Username = "AnotherUser", PasswordHash = "anotherPass" };
			var dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
			var mockSet = new Mock<DbSet<User>>();
			dbContextMock.Setup(db => db.Users).Returns(mockSet.Object);
			dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(0);

			var authServiceMock = new AuthenticationService(dbContextMock.Object, _config);

			// Act
			var result = await authServiceMock.RegisterUserAsync(newUser);

			// Assert
			result.Should().BeFalse();
			mockSet.Verify(db => db.AddAsync(It.Is<User>(u => u == newUser), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
			dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
