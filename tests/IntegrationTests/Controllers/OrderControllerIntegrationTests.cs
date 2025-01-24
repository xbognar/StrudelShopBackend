using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using DataAccess.DTOs;
using System.Collections.Generic;
using System;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for OrderController endpoints.
	/// </summary>
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
			// ACT
			var response = await _client.GetAsync("/api/order");
			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		/// <summary>
		/// Ensures an admin user can retrieve all orders successfully.
		/// </summary>
		[Fact]
		public async Task GetAllOrders_AsAdmin_ReturnsOkAndList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
			orders.Should().NotBeNull();
			orders.Should().HaveCountGreaterThan(0);
		}

		/// <summary>
		/// Ensures a user can fetch an existing order (#1000 seeded) by ID with success.
		/// </summary>
		[Fact]
		public async Task GetOrderById_AsUser_ReturnsOkIfExists()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order/1000");

			// ASSERT
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
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order/999999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}


		/// <summary>
		/// Tests that an admin can delete an existing order (#1001), returning NoContent or NotFound if absent.
		/// </summary>
		[Fact]
		public async Task DeleteOrder_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/order/1001");

			// ASSERT
			response.StatusCode.Should().BeOneOf(
				new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound },
				$"Expected 204 NoContent or 404 NotFound but got {response.StatusCode}"
			);
		}

		/// <summary>
		/// Tests fetching a user's order history (for user #2) returns OK and non-empty list.
		/// </summary>
		[Fact]
		public async Task GetOrderHistory_AsUser_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order/history/2");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var history = await response.Content.ReadFromJsonAsync<List<OrderHistoryDTO>>();
			history.Should().NotBeNull();
			history.Should().HaveCountGreaterThan(0);
		}

		/// <summary>
		/// Tests fetching detailed info about an order (#1000) as admin returns the expected details object.
		/// </summary>
		[Fact]
		public async Task GetOrderDetails_AsAdmin_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order/details/1000");

			// ASSERT
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
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/order/summary");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var summaries = await response.Content.ReadFromJsonAsync<List<CustomerOrderSummaryDTO>>();
			summaries.Should().NotBeNull();
			summaries.Should().HaveCountGreaterThan(0);
		}
	}
}
