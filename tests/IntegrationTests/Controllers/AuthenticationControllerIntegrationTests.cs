using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using DataAccess.DTOs;

namespace IntegrationTests.Controllers
{
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
			var newUser = new User
			{
				Username = "newTestUser",
				PasswordHash = "testPass",
				Email = "newuser@mail.com"
			};

			var response = await _client.PostAsJsonAsync("/api/authentication/register", newUser);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var message = await response.Content.ReadAsStringAsync();
			message.Should().Contain("Registration successful");
		}

		/// <summary>
		/// Tests that an invalid registration (e.g., missing password) returns a BadRequest.
		/// </summary>
		[Fact]
		public async Task Register_FailsIfPasswordMissing_ReturnsBadRequest()
		{
			var newUser = new User
			{
				Username = "noPasswordUser",
				PasswordHash = "",
				Email = "nopassword@mail.com"
			};

			var response = await _client.PostAsJsonAsync("/api/authentication/register", newUser);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests that logging in with correct admin credentials succeeds and returns a token.
		/// </summary>
		[Fact]
		public async Task Login_ValidAdminCredentials_ReturnsOkAndToken()
		{
			var loginRequest = new LoginRequestDTO
			{
				Username = "admin",
				Password = "adminPass"
			};

			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);

			response.StatusCode.Should().Be(HttpStatusCode.OK);

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
			var loginRequest = new LoginRequestDTO
			{
				Username = "john",
				Password = "johnPass"
			};

			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);

			response.StatusCode.Should().Be(HttpStatusCode.OK);

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
			var loginRequest = new LoginRequestDTO
			{
				Username = "fakeUser",
				Password = "wrongPass"
			};

			var response = await _client.PostAsJsonAsync("/api/authentication/login", loginRequest);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
			var msg = await response.Content.ReadAsStringAsync();
			msg.Should().Contain("Invalid credentials");
		}
	}
}
