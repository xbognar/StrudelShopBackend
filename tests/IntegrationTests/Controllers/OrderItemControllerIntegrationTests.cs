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
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/orderitem");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var items = await response.Content.ReadFromJsonAsync<List<OrderItem>>();
			items.Should().NotBeNull();
			items.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Tests fetching a specific order item by ID as a user returns OK if it exists.
		/// </summary>
		[Fact]
		public async Task GetOrderItemById_AsUser_ReturnsOkIfExists()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/orderitem/500");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var item = await response.Content.ReadFromJsonAsync<OrderItem>();
			item.Should().NotBeNull();
			item.OrderItemID.Should().Be(500);
		}

		/// <summary>
		/// Tests that a non-existent order item returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetOrderItemById_NotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/orderitem/999999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new order item as a user returns Created status.
		/// </summary>
		[Fact]
		public async Task CreateOrderItem_AsUser_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var newItem = new OrderItem
			{
				OrderItemID = 888,
				OrderID = 1000,
				ProductID = 10,
				Quantity = 3,
				Price = 5.99m
			};

			var response = await _client.PostAsJsonAsync("/api/orderitem", newItem);
			response.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		/// <summary>
		/// Tests that an admin can update an order item successfully.
		/// </summary>
		[Fact]
		public async Task UpdateOrderItem_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updated = new OrderItem
			{
				OrderItemID = 500,
				OrderID = 1000,
				ProductID = 10,
				Quantity = 5,
				Price = 5.99m
			};

			var response = await _client.PutAsJsonAsync("/api/orderitem/500", updated);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests that if the path ID does not match the object's ID, a BadRequest is returned.
		/// </summary>
		[Fact]
		public async Task UpdateOrderItem_IdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var item = new OrderItem
			{
				OrderItemID = 501,
				OrderID = 1000,
				ProductID = 11,
				Quantity = 2
			};

			var response = await _client.PutAsJsonAsync("/api/orderitem/9999", item);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests that an admin can delete an order item, returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteOrderItem_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/orderitem/502");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}
	}
}
