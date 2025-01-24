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
	/// <summary>
	/// Integration tests for ProductController endpoints.
	/// </summary>
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
			// ACT
			var response = await _client.GetAsync("/api/product");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var products = await response.Content.ReadFromJsonAsync<List<Product>>();
			products.Should().NotBeNull();
			products.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Verifies that retrieving a product by ID=10 returns Ok if it exists (Apple Strudel).
		/// </summary>
		[Fact]
		public async Task GetProductById_Found_ReturnsOk()
		{
			// ARRANGE - no login needed
			// ACT
			var response = await _client.GetAsync("/api/product/10");

			// ASSERT
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
			// ACT
			var response = await _client.GetAsync("/api/product/99999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Verifies that an admin can create a new product successfully (201 Created).
		/// </summary>
		[Fact]
		public async Task CreateProduct_AsAdmin_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newProduct = new Product
			{
				Name = "Test Product",
				Description = "Test description",
				Price = 9.99m,
				ImageURL = "test_product.jpg", 
				ProductImages = new List<ProductImage>() 
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/product", newProduct);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.Created)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful product creation, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
			var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
			createdProduct.Should().NotBeNull();
			createdProduct.Name.Should().Be("Test Product");
			createdProduct.Description.Should().Be("Test description");
			createdProduct.Price.Should().Be(9.99m);
			createdProduct.ImageURL.Should().Be("test_product.jpg");
		}

		/// <summary>
		/// Verifies that a normal user cannot create a new product (should return Forbidden).
		/// </summary>
		[Fact]
		public async Task CreateProduct_AsUser_ReturnsForbidden()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "john", "johnPass");
			_client.AddAuthToken(token);

			var product = new Product
			{
				Name = "Not Allowed",
				Description = "Should not be created",
				Price = 15.99m,
				ImageURL = "not_allowed.jpg",
				ProductImages = new List<ProductImage>()
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/product", product);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		/// <summary>
		/// Tests that updating a product (#10) as admin returns NoContent on success.
		/// </summary>
		[Fact]
		public async Task UpdateProduct_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedProd = new Product
			{
				ProductID = 10,
				Name = "Apple Strudel Updated",
				Description = "Updated description",
				Price = 6.50m,
				ImageURL = "apple_strudel_updated.jpg",
				ProductImages = new List<ProductImage>() 
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/product/10", updatedProd);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.NoContent)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful product update, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.NoContent, $"Expected 204 NoContent but got {response.StatusCode}");
		}


		/// <summary>
		/// Verifies that an admin can delete an existing product (#11), returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteProduct_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/product/11");

			// ASSERT
			response.StatusCode.Should().BeOneOf(
			HttpStatusCode.NoContent,
			HttpStatusCode.NotFound
			);
		}

		/// <summary>
		/// Verifies that any user can retrieve top-selling products without authentication.
		/// </summary>
		[Fact]
		public async Task GetTopSellingProducts_AllowAnonymous_ReturnsOk()
		{
			// ACT
			var response = await _client.GetAsync("/api/product/top-selling");

			// ASSERT
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
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/product/overview");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var overview = await response.Content.ReadFromJsonAsync<List<ProductOverviewDTO>>();
			overview.Should().NotBeNull();
			overview.Should().HaveCountGreaterThan(0);
		}
	}
}
