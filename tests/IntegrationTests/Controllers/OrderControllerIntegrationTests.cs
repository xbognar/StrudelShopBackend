using System.Collections.Generic;
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
	public class OrderControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public OrderControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Ensures that a request to get all orders without authentication fails with Unauthorized.
		/// </summary>
		[Fact]
		public async Task GetAllOrders_AsAnonymous_ReturnsUnauthorized()
		{
			var response = await _client.GetAsync("/api/order");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		/// <summary>
		/// Ensures an admin user can retrieve all orders successfully.
		/// </summary>
		[Fact]
		public async Task GetAllOrders_AsAdmin_ReturnsOkAndList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
			orders.Should().NotBeNull();
			orders.Should().HaveCountGreaterThan(0);
		}

		/// <summary>
		/// Ensures a user can fetch an existing order by ID with success.
		/// </summary>
		[Fact]
		public async Task GetOrderById_AsUser_ReturnsOkIfExists()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order/1000");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var order = await response.Content.ReadFromJsonAsync<Order>();
			order.Should().NotBeNull();
			order.OrderID.Should().Be(1000);
		}

		/// <summary>
		/// Ensures that fetching a non-existent order returns NotFound for a user.
		/// </summary>
		[Fact]
		public async Task GetOrderById_AsUser_WhenNotFound_ReturnsNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order/999999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new order as a user returns CreatedAtAction.
		/// </summary>
		[Fact]
		public async Task CreateOrder_AsUser_ReturnsCreatedAtAction()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var newOrder = new Order
			{
				OrderID = 2000,
				UserID = 2,
				TotalAmount = 20.00m,
				PaymentStatus = "Pending"
			};

			var response = await _client.PostAsJsonAsync("/api/order", newOrder);

			response.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		/// <summary>
		/// Tests updating an existing order as an admin role returns NoContent on success.
		/// </summary>
		[Fact]
		public async Task UpdateOrder_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedOrder = new Order
			{
				OrderID = 1000,
				UserID = 2,
				PaymentStatus = "Paid",
				TotalAmount = 99.99m
			};

			var response = await _client.PutAsJsonAsync("/api/order/1000", updatedOrder);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Ensures that if the route ID and body ID do not match, a BadRequest is returned.
		/// </summary>
		[Fact]
		public async Task UpdateOrder_IdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var order = new Order
			{
				OrderID = 1000,
				UserID = 2
			};

			var response = await _client.PutAsJsonAsync("/api/order/9999", order);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests that an admin can delete an existing order, returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteOrder_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/order/1001");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests fetching a user's order history returns OK and a list of OrderHistoryDTO.
		/// </summary>
		[Fact]
		public async Task GetOrderHistory_AsUser_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order/history/2");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var history = await response.Content.ReadFromJsonAsync<List<OrderHistoryDTO>>();
			history.Should().NotBeNull();
			history.Should().HaveCountGreaterThan(0);
		}

		/// <summary>
		/// Tests fetching detailed info about an order as admin returns the expected details object.
		/// </summary>
		[Fact]
		public async Task GetOrderDetails_AsAdmin_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order/details/1000");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var details = await response.Content.ReadFromJsonAsync<OrderDetailsDTO>();
			details.Should().NotBeNull();
			details.OrderId.Should().Be(1000);
			details.OrderItems.Should().NotBeEmpty();
		}

		/// <summary>
		/// Tests the admin-only endpoint for getting a summary of all customer orders.
		/// </summary>
		[Fact]
		public async Task GetCustomerOrderSummaries_AsAdmin_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/order/summary");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var summaries = await response.Content.ReadFromJsonAsync<List<CustomerOrderSummaryDTO>>();
			summaries.Should().NotBeNull();
			summaries.Should().HaveCountGreaterThan(0);
		}
	}
}
