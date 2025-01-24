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

		/// <summary>
		/// Tests creating a new order item as a user returns Created status.
		/// </summary>
		[Fact]
		public async Task CreateOrderItem_AsUser_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var newItem = new OrderItem
			{
				OrderID = 1000,        
				ProductID = 10,          
				Quantity = 3,            
				Price = 5.99m            
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/orderitem", newItem);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.Created)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful order item creation, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
			var createdItem = await response.Content.ReadFromJsonAsync<OrderItem>();
			createdItem.Should().NotBeNull();
			createdItem.OrderID.Should().Be(1000);
			createdItem.ProductID.Should().Be(10);
			createdItem.Quantity.Should().Be(3);
			createdItem.Price.Should().Be(5.99m);
		}

		/// <summary>
		/// Tests that an admin can update an order item (#500) successfully.
		/// </summary>
		[Fact]
		public async Task UpdateOrderItem_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
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

			// ACT
			var response = await _client.PutAsJsonAsync("/api/orderitem/500", updated);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.NoContent)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful order item update, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.NoContent, $"Expected 204 NoContent but got {response.StatusCode}");
		}
	}
}
