using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using System.Collections.Generic;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for OrderItemController endpoints.
	/// </summary>
	public class OrderItemControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public OrderItemControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests that an admin can retrieve all order items successfully.
		/// </summary>
		[Fact]
		public async Task GetAllOrderItems_AsAdmin_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/orderitem");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var items = await response.Content.ReadFromJsonAsync<List<OrderItem>>();
			items.Should().NotBeNull();
			items.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Tests fetching a specific order item (#500) by ID as a user returns OK if it exists.
		/// </summary>
		[Fact]
		public async Task GetOrderItemById_AsUser_ReturnsOkIfExists()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/orderitem/500");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var item = await response.Content.ReadFromJsonAsync<OrderItem>();
			item.Should().NotBeNull();
			item.OrderItemID.Should().Be(500);
		}

		/// <summary>
		/// Tests that a non-existent order item returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetOrderItemById_NotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/orderitem/999999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
