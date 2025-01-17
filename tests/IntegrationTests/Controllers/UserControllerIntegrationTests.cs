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
	public class UserControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public UserControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Verifies that an anonymous request to get users returns Unauthorized due to admin-only requirement.
		/// </summary>
		[Fact]
		public async Task GetAllUsers_AsAnonymous_ReturnsUnauthorized()
		{
			var response = await _client.GetAsync("/api/user");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		/// <summary>
		/// Ensures that an admin can retrieve all users successfully.
		/// </summary>
		[Fact]
		public async Task GetAllUsers_AsAdmin_ReturnsOkAndList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/user");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var users = await response.Content.ReadFromJsonAsync<List<User>>();
			users.Should().NotBeNull();
			users.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Ensures that an admin can retrieve a user by ID if it exists.
		/// </summary>
		[Fact]
		public async Task GetUserById_AsAdmin_WhenFound_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/user/2");
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
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/user/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Ensures that an admin can create a new user, returning Created.
		/// </summary>
		[Fact]
		public async Task CreateUser_AsAdmin_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newUser = new User
			{
				UserID = 12345,
				Username = "testUser999",
				PasswordHash = "pass999",
				Role = "User",
				Email = "test999@strudel.com"
			};

			var response = await _client.PostAsJsonAsync("/api/user", newUser);
			response.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		/// <summary>
		/// Ensures that an admin can update an existing user's details, returning NoContent on success.
		/// </summary>
		[Fact]
		public async Task UpdateUser_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedUser = new User
			{
				UserID = 2,
				Username = "john",
				PasswordHash = "johnPass",
				Role = "User",
				Email = "updatedjohn@strudel.com",
				Address = "New address"
			};

			var response = await _client.PutAsJsonAsync("/api/user/2", updatedUser);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Ensures that if the ID in the route does not match the user's ID in the body, a BadRequest is returned.
		/// </summary>
		[Fact]
		public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var user = new User
			{
				UserID = 2,
				Username = "john"
			};

			var response = await _client.PutAsJsonAsync("/api/user/9999", user);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Ensures that an admin can delete a user, returning NoContent or NotFound if it doesn't exist.
		/// </summary>
		[Fact]
		public async Task DeleteUser_AsAdmin_ReturnsNoContentOrNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/user/12345");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}
	}
}
