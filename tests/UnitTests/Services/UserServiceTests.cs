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
	public class UserServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<User>> _userDbSetMock;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			// Create an in-memory DbContextOptions just to pass into the mock constructor
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			// Mock the DbSet<User> so we can arrange data
			_userDbSetMock = new Mock<DbSet<User>>();
			_dbContextMock.Setup(db => db.Users).Returns(_userDbSetMock.Object);

			// Instantiate the UserService with the mocked context
			_userService = new UserService(_dbContextMock.Object);
		}

		[Fact]
		public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
		{
			// Arrange
			var testUser = new User { UserID = 1, Username = "Alice" };
			_dbContextMock
				.Setup(db => db.Users.FindAsync(1))
				.ReturnsAsync(testUser);

			// Act
			var result = await _userService.GetUserByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.UserID.Should().Be(1);
			result.Username.Should().Be("Alice");
		}

		[Fact]
		public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock
				.Setup(db => db.Users.FindAsync(999))
				.ReturnsAsync((User)null);

			// Act
			var result = await _userService.GetUserByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsAllUsers()
		{
			// Arrange
			var testUsers = new List<User>
			{
				new User { UserID = 1, Username = "Alice" },
				new User { UserID = 2, Username = "Bob" }
			}.AsQueryable();

			// Setup the IQueryable behavior
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(testUsers.Provider);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(testUsers.Expression);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(testUsers.ElementType);
			_userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(testUsers.GetEnumerator());

			// Act
			var result = await _userService.GetAllUsersAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task CreateUserAsync_SavesUserAndCallsSaveChanges()
		{
			// Arrange
			var newUser = new User { UserID = 3, Username = "Charlie" };

			// Act
			await _userService.CreateUserAsync(newUser);

			// Assert
			_dbContextMock.Verify(db => db.Users.AddAsync(newUser, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task UpdateUserAsync_UpdatesUserAndCallsSaveChanges()
		{
			// Arrange
			var existingUser = new User { UserID = 10, Username = "OldName" };

			// Act
			await _userService.UpdateUserAsync(existingUser);

			// Assert
			_dbContextMock.Verify(db => db.Users.Update(existingUser), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteUserAsync_WhenUserExists_DeletesUser()
		{
			// Arrange
			var user = new User { UserID = 5, Username = "DeleteMe" };
			_dbContextMock.Setup(db => db.Users.FindAsync(5)).ReturnsAsync(user);

			// Act
			await _userService.DeleteUserAsync(5);

			// Assert
			_dbContextMock.Verify(db => db.Users.Remove(user), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		[Fact]
		public async Task DeleteUserAsync_WhenUserDoesNotExist_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Users.FindAsync(999)).ReturnsAsync((User)null);

			// Act
			await _userService.DeleteUserAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.Users.Remove(It.IsAny<User>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}
	}
}
