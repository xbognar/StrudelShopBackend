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
	public class ProductControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public ProductControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Verifies that any user (anonymous) can retrieve all products successfully.
		/// </summary>
		[Fact]
		public async Task GetAllProducts_AllowAnonymous_ReturnsOk()
		{
			var response = await _client.GetAsync("/api/product");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var products = await response.Content.ReadFromJsonAsync<List<Product>>();
			products.Should().NotBeNull();
			products.Count.Should().Be(2);
		}

		/// <summary>
		/// Verifies that retrieving a product by ID returns Ok if the product exists.
		/// </summary>
		[Fact]
		public async Task GetProductById_Found_ReturnsOk()
		{
			var response = await _client.GetAsync("/api/product/10");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var product = await response.Content.ReadFromJsonAsync<Product>();
			product.Should().NotBeNull();
			product.Name.Should().Be("Apple Strudel");
		}

		/// <summary>
		/// Verifies that retrieving a product by an unknown ID returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetProductById_NotFound_ReturnsNotFound()
		{
			var response = await _client.GetAsync("/api/product/99999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Verifies that an admin can create a new product successfully.
		/// </summary>
		[Fact]
		public async Task CreateProduct_AsAdmin_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newProduct = new Product
			{
				ProductID = 99,
				Name = "Test Product",
				Description = "Test description",
				Price = 9.99m
			};

			var response = await _client.PostAsJsonAsync("/api/product", newProduct);
			response.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		/// <summary>
		/// Verifies that a normal user cannot create a new product (should return Forbidden).
		/// </summary>
		[Fact]
		public async Task CreateProduct_AsUser_ReturnsForbidden()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var product = new Product
			{
				ProductID = 123,
				Name = "Not Allowed"
			};

			var response = await _client.PostAsJsonAsync("/api/product", product);
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		/// <summary>
		/// Tests that updating a product as admin returns NoContent on success.
		/// </summary>
		[Fact]
		public async Task UpdateProduct_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedProd = new Product
			{
				ProductID = 10, // existing
				Name = "Apple Strudel Updated",
				Price = 6.50m
			};

			var response = await _client.PutAsJsonAsync("/api/product/10", updatedProd);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Ensures that if the path ID does not match the product's ID, a BadRequest is returned.
		/// </summary>
		[Fact]
		public async Task UpdateProduct_IdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var product = new Product
			{
				ProductID = 10,
				Name = "Mismatch"
			};

			var response = await _client.PutAsJsonAsync("/api/product/9999", product);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Verifies that an admin can delete an existing product, returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteProduct_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/product/11");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Verifies that any user can retrieve top-selling products without authentication.
		/// </summary>
		[Fact]
		public async Task GetTopSellingProducts_AllowAnonymous_ReturnsOk()
		{
			var response = await _client.GetAsync("/api/product/top-selling");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var topSelling = await response.Content.ReadFromJsonAsync<List<TopSellingProductDTO>>();
			topSelling.Should().NotBeNull();
			topSelling.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Verifies that an admin can get an overview of products, returning OK and data.
		/// </summary>
		[Fact]
		public async Task GetProductOverview_AsAdmin_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/product/overview");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var overview = await response.Content.ReadFromJsonAsync<List<ProductOverviewDTO>>();
			overview.Should().NotBeNull();
			overview.Should().HaveCountGreaterThan(0);
		}
	}
}
