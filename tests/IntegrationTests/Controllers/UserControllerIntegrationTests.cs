// IntegrationTests/Controllers/UserControllerIntegrationTests.cs
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IntegrationTests.Dependencies;
using DataAccess.Models;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for UserController endpoints.
	/// </summary>
	public class UserControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public UserControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Verifies that an anonymous request to get users returns Unauthorized (admin-only route).
		/// </summary>
		[Fact]
		public async Task GetAllUsers_AsAnonymous_ReturnsUnauthorized()
		{
			// ARRANGE - no login
			// ACT
			var response = await _client.GetAsync("/api/user");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		/// <summary>
		/// Ensures that an admin can retrieve all users successfully.
		/// </summary>
		[Fact]
		public async Task GetAllUsers_AsAdmin_ReturnsOkAndList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/user");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var users = await response.Content.ReadFromJsonAsync<List<User>>();
			users.Should().NotBeNull();
			users.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Ensures that an admin can retrieve a user by ID (#2) if it exists.
		/// </summary>
		[Fact]
		public async Task GetUserById_AsAdmin_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/user/2");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var user = await response.Content.ReadFromJsonAsync<User>();
			user.Should().NotBeNull();
			user.Username.Should().Be("john");
		}

		/// <summary>
		/// Ensures that NotFound is returned if a user doesn't exist.
		/// </summary>
		[Fact]
		public async Task GetUserById_NotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/user/9999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Ensures that an admin can create a new user, returning Created.
		/// </summary>
		[Fact]
		public async Task CreateUser_AsAdmin_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newUser = new User
			{
				Username = "testUser999",
				PasswordHash = "pass999",
				Role = "User",
				Email = "test999@strudel.com",
				FirstName = "Test",
				LastName = "User",
				PhoneNumber = "9998887777",
				Address = "999 Test Street",
				Orders = new List<Order>()
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/user", newUser);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.Created)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful user creation, but got errors: {errorContent}");
			}

			response.StatusCode.Should().Be(HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
			var createdUser = await response.Content.ReadFromJsonAsync<User>();
			createdUser.Should().NotBeNull();
			createdUser.Username.Should().Be("testUser999");
			createdUser.Email.Should().Be("test999@strudel.com");
			createdUser.Role.Should().Be("User");
		}

		/// <summary>
		/// Ensures that an admin can update an existing user's details (#2).
		/// </summary>
		[Fact]
		public async Task UpdateUser_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedUser = new User
			{
				UserID = 2,
				Username = "john",
				PasswordHash = "johnPass",
				Role = "User",
				Email = "updatedjohn@strudel.com",
				Address = "New Address Street 42",
				FirstName = "John",
				LastName = "Updated",
				PhoneNumber = "654321",
				Orders = new List<Order>()
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/user/2", updatedUser);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.NoContent)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful user update, but got errors: {errorContent}");
			}

			response.StatusCode.Should().Be(HttpStatusCode.NoContent, $"Expected 204 NoContent but got {response.StatusCode}");
		}


		/// <summary>
		/// Ensures that an admin can delete a user, returning NoContent or NotFound if it doesn't exist.
		/// </summary>
		[Fact]
		public async Task DeleteUser_AsAdmin_ReturnsNoContentOrNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/user/12345");

			// ASSERT
			response.StatusCode.Should().BeOneOf(
			HttpStatusCode.NoContent,
			HttpStatusCode.NotFound
			);
		}
	}
}
