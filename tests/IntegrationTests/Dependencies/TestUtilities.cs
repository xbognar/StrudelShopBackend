using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DataAccess.DTOs;
using FluentAssertions;
using System.Net;

namespace IntegrationTests.Dependencies
{
	public static class TestUtilities
	{
		/// <summary>
		/// Logs in with the given username + password, returning the JWT token string if successful.
		/// </summary>
		public static async Task<string> LoginAndGetTokenAsync(HttpClient client, string username, string password)
		{
			var loginRequest = new LoginRequestDTO
			{
				Username = username,
				Password = password
			};

			var response = await client.PostAsJsonAsync("/api/authentication/login", loginRequest);
			response.StatusCode.Should().Be(HttpStatusCode.OK, "Login should succeed if credentials are correct");

			var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
			loginResponse.Should().NotBeNull();
			loginResponse.Token.Should().NotBeNullOrEmpty();

			return loginResponse.Token;
		}

		/// <summary>
		/// Adds a Bearer token to the HttpClient's DefaultRequestHeaders for subsequent requests.
		/// </summary>
		public static void AddAuthToken(this HttpClient client, string token)
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}
	}
}
