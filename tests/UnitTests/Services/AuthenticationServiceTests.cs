using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataAccess.Services;
using DataAccess.Models;
using DataAccess.DTOs;
using StrudelShop.DataAccess.DataAccess;

namespace UnitTests.Services
{
	public class AuthenticationServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<User>> _userDbSetMock;
		private readonly Mock<IConfiguration> _configMock;
		private readonly AuthenticationService _authService;

		public AuthenticationServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_userDbSetMock = new Mock<DbSet<User>>();
			_dbContextMock.Setup(db => db.Users).Returns(_userDbSetMock.Object);

			_configMock = new Mock<IConfiguration>();
			_configMock.SetupGet(x => x["ADMIN_USERNAME"]).Returns("admin");
			_configMock.SetupGet(x => x["ADMIN_PASSWORD"]).Returns("adminPass");
			_configMock.SetupGet(x => x["JWT_KEY"]).Returns("SomeSuperSecretKey");
			_configMock.SetupGet(x => x["JWT_TOKEN_EXPIRY_MINUTES"]).Returns("30");
			_configMock.SetupGet(x => x["JWT_ISSUER"]).Returns("TestIssuer");
			_configMock.SetupGet(x => x["JWT_AUDIENCE"]).Returns("TestAudience");

			_authService = new AuthenticationService(_dbContextMock.Object, _configMock.Object);
		}

		[Fact]
		public async Task AuthenticateAsync_AdminCredentials_ReturnsAdminLoginResponse()
		{
			// Arrange
			var adminUsername = "admin";
			var adminPassword = "adminPass";

			// Act
			var result = await _authService.AuthenticateAsync(adminUsername, adminPassword);

			// Assert
			result.Should().NotBeNull();
			result.Role.Should().Be("Admin");
			result.Token.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task AuthenticateAsync_ValidUserCredentials_ReturnsUserLoginResponse()
		{
			// Arrange
			var username = "testUser";
			var password = "testPass";

			var userData = new List<User>
			{
				new User
				{
					UserID = 1,
					Username = "testUser",
					PasswordHash = "testPass",
                    Role = "User",
					FirstName = "Test",
					LastName = "User"
				}
			}.AsQueryable();

			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

			// Act
			var result = await _authService.AuthenticateAsync(username, password);

			// Assert
			result.Should().NotBeNull();
			result.UserId.Should().Be(1);
			result.Role.Should().Be("User");
			result.Token.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
		{
			// Arrange
			var username = "testUser";
			var password = "wrongPass";

			var userData = new List<User>
			{
				new User
				{
					UserID = 1,
					Username = "testUser",
					PasswordHash = "testPass",
					Role = "User"
				}
			}.AsQueryable();

			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

			// Act
			var result = await _authService.AuthenticateAsync(username, password);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task RegisterUserAsync_SuccessfulSave_ReturnsTrue()
		{
			// Arrange
			var newUser = new User { Username = "NewUser", PasswordHash = "plainText" };

			// Act
			var result = await _authService.RegisterUserAsync(newUser);

			// Assert
			result.Should().BeTrue();
			_dbContextMock.Verify(db => db.Users.AddAsync(newUser, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
			newUser.PasswordHash.Should().Be("plainText");
			newUser.Role.Should().Be("User");
		}

		[Fact]
		public async Task RegisterUserAsync_NoChangesSaved_ReturnsFalse()
		{
			// Arrange
			var newUser = new User { Username = "NewUser", PasswordHash = "plainText" };
			_dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(0);

			// Act
			var result = await _authService.RegisterUserAsync(newUser);

			// Assert
			result.Should().BeFalse();
		}
	}
}
