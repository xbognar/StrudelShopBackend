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

namespace UnitTests.Services
{
	public class UserServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			// Setup In-Memory Database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: $"UserTestDb_{Guid.NewGuid()}")
				.Options;

			_dbContext = new ApplicationDbContext(options);

			// Seed initial data
			var user1 = new User
			{
				UserID = 1,
				Username = "Alice",
				PasswordHash = "alicePass",
				Role = "User",
				FirstName = "Alice",
				LastName = "Wonderland",
				Email = "alice@strudelshop.com",
				PhoneNumber = "1112223333",
				Address = "123 Apple St"
			};
			var user2 = new User
			{
				UserID = 2,
				Username = "Bob",
				PasswordHash = "bobPass",
				Role = "User",
				FirstName = "Bob",
				LastName = "Builder",
				Email = "bob@strudelshop.com",
				PhoneNumber = "4445556666",
				Address = "456 Banana Ave"
			};

			_dbContext.Users.AddRange(user1, user2);
			_dbContext.SaveChanges();

			_userService = new UserService(_dbContext);
		}

		/// <summary>
		/// Tests that GetUserByIdAsync returns the user when the user is found.
		/// </summary>
		[Fact]
		public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
		{
			// Arrange
			var userId = 1;

			// Act
			var result = await _userService.GetUserByIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.UserID.Should().Be(userId);
			result.Username.Should().Be("Alice");
			result.FirstName.Should().Be("Alice");
			result.LastName.Should().Be("Wonderland");
			result.Email.Should().Be("alice@strudelshop.com");
			result.PhoneNumber.Should().Be("1112223333");
			result.Address.Should().Be("123 Apple St");
			result.Role.Should().Be("User");
		}

		/// <summary>
		/// Tests that GetUserByIdAsync returns null when the user does not exist.
		/// </summary>
		[Fact]
		public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
		{
			// Arrange
			var userId = 999;

			// Act
			var result = await _userService.GetUserByIdAsync(userId);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllUsersAsync returns the full list of users in the DbSet.
		/// </summary>
		[Fact]
		public async Task GetAllUsersAsync_ReturnsAllUsers()
		{
			// Act
			var result = await _userService.GetAllUsersAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(u => u.UserID == 1 && u.Username == "Alice");
			result.Should().Contain(u => u.UserID == 2 && u.Username == "Bob");
		}

		/// <summary>
		/// Tests that CreateUserAsync adds a new user and calls SaveChanges once.
		/// </summary>
		[Fact]
		public async Task CreateUserAsync_SavesUserAndCallsSaveChanges()
		{
			// Arrange
			var newUser = new User
			{
				UserID = 3,
				Username = "Charlie",
				PasswordHash = "charliePass",
				Role = "User",
				FirstName = "Charlie",
				LastName = "Chaplin",
				Email = "charlie@strudelshop.com",
				PhoneNumber = "7778889999",
				Address = "789 Cherry Blvd"
			};

			// Act
			await _userService.CreateUserAsync(newUser);

			// Assert
			var createdUser = await _dbContext.Users.FindAsync(3);
			createdUser.Should().NotBeNull();
			createdUser.Username.Should().Be("Charlie");
			createdUser.FirstName.Should().Be("Charlie");
			createdUser.LastName.Should().Be("Chaplin");
			createdUser.Email.Should().Be("charlie@strudelshop.com");
			createdUser.PhoneNumber.Should().Be("7778889999");
			createdUser.Address.Should().Be("789 Cherry Blvd");
			createdUser.Role.Should().Be("User");
		}

		/// <summary>
		/// Tests that UpdateUserAsync updates the user entity and calls SaveChanges once.
		/// </summary>
		[Fact]
		public async Task UpdateUserAsync_UpdatesUserAndCallsSaveChanges()
		{
			// Arrange
			var existingUser = await _dbContext.Users.FindAsync(1);
			existingUser.FirstName = "Alicia";
			existingUser.LastName = "Wonder";

			// Act
			await _userService.UpdateUserAsync(existingUser);

			// Assert
			var updatedUser = await _dbContext.Users.FindAsync(1);
			updatedUser.FirstName.Should().Be("Alicia");
			updatedUser.LastName.Should().Be("Wonder");
		}

		/// <summary>
		/// Tests that DeleteUserAsync removes the user if it is found.
		/// </summary>
		[Fact]
		public async Task DeleteUserAsync_WhenUserExists_DeletesUser()
		{
			// Arrange
			var userId = 2;
			var existingUser = await _dbContext.Users.FindAsync(userId);

			// Act
			await _userService.DeleteUserAsync(userId);

			// Assert
			var deletedUser = await _dbContext.Users.FindAsync(userId);
			deletedUser.Should().BeNull();
		}

		/// <summary>
		/// Tests that DeleteUserAsync does nothing if the user is not found.
		/// </summary>
		[Fact]
		public async Task DeleteUserAsync_WhenUserDoesNotExist_DoesNothing()
		{
			// Arrange
			var nonExistentUserId = 999;

			// Act
			await _userService.DeleteUserAsync(nonExistentUserId);

			// Assert
			var user = await _dbContext.Users.FindAsync(nonExistentUserId);
			user.Should().BeNull();
			_dbContext.Users.Count().Should().Be(2); // Ensure no users were removed
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
