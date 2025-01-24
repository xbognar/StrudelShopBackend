using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using DataAccess.DTOs;
using System.Collections.Generic;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for the AuthenticationController endpoints.
	/// </summary>
	public class AuthenticationControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public AuthenticationControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests if registering a valid new user returns OK and the success message.
		/// </summary>
		[Fact]
		public async Task Register_ValidUser_ReturnsOk()
		{
			// ARRANGE
			var newUser = new User
			{
				Username = "newTestUser",
				PasswordHash = "testPass123",
				Email = "newuser@mail.com",
				FirstName = "New",
				LastName = "User",
				PhoneNumber = "1112223333",
				Address = "123 Test Street",
				Role = "User",
				Orders = new List<Order>()
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/authentication/register", newUser);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.OK)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful registration, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode}");
			var message = await response.Content.ReadAsStringAsync();
			message.Should().Contain("Registration successful");
		}

		
		/// <summary>
		/// Tests that logging in with correct admin credentials succeeds and returns a token.
		/// </summary>
		[Fact]
		public async Task Login_ValidAdminCredentials_ReturnsOkAndToken()
		{
			// ARRANGE
			var loginRequest = new LoginRequestDTO
			{
				Username = "admin",
				Password = "adminPass"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode}");
			var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
			loginResponse.Should().NotBeNull();
			loginResponse.Token.Should().NotBeNullOrEmpty();
			loginResponse.Role.Should().Be("Admin");
		}

		/// <summary>
		/// Tests that logging in as a valid normal user succeeds and returns a token with the "User" role.
		/// </summary>
		[Fact]
		public async Task Login_ValidUserCredentials_ReturnsOk()
		{
			// ARRANGE
			var loginRequest = new LoginRequestDTO
			{
				Username = "john",
				Password = "johnPass"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode}");
			var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
			loginResponse.Should().NotBeNull();
			loginResponse.Token.Should().NotBeNullOrEmpty();
			loginResponse.Role.Should().Be("User");
		}

		/// <summary>
		/// Tests that an invalid login attempt returns Unauthorized.
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// ARRANGE
			var loginRequest = new LoginRequestDTO
			{
				Username = "fakeUser",
				Password = "wrongPass"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, $"Expected 401 Unauthorized but got {response.StatusCode}");
			var msg = await response.Content.ReadAsStringAsync();
			msg.Should().Contain("Invalid credentials");
		}
	}
}
