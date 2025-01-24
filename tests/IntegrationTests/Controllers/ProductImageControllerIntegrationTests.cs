// IntegrationTests/Controllers/ProductImageControllerIntegrationTests.cs
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
	/// <summary>
	/// Integration tests for ProductImageController endpoints.
	/// </summary>
	public class ProductImageControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public ProductImageControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Verifies that an admin can retrieve all product images, returning OK.
		/// </summary>
		[Fact]
		public async Task GetAllProductImages_AsAdmin_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/productimage");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var images = await response.Content.ReadFromJsonAsync<List<ProductImage>>();
			images.Should().NotBeNull();
			images.Should().HaveCount(2);
		}

		/// <summary>
		/// Verifies fetching a product image #100 by a known ID returns OK for admin.
		/// </summary>
		[Fact]
		public async Task GetProductImageById_AsAdmin_ReturnsOkIfExists()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/productimage/100");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var image = await response.Content.ReadFromJsonAsync<ProductImage>();
			image.Should().NotBeNull();
			image.ImageURL.Should().Be("apple_strudel_1.jpg");
		}

		/// <summary>
		/// Verifies that a non-existent product image returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetProductImageById_NotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/productimage/9999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Verifies that an admin can create a new product image successfully.
		/// </summary>
		[Fact]
		public async Task CreateProductImage_AsAdmin_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newImage = new ProductImage
			{
				ProductID = 10,
				ImageURL = "apple_new_side.jpg",
				Product = new Product
				{
					ProductID = 10,
					Name = "Apple Strudel",
					Description = "Delicious apple pastry",
					Price = 5.99m,
					ImageURL = "apple.jpg",
					ProductImages = new List<ProductImage>()
				}
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/productimage", newImage);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.Created)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful product image creation, but got errors: {errorContent}");
			}
			response.StatusCode.Should().Be(HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
			var createdImage = await response.Content.ReadFromJsonAsync<ProductImage>();
			createdImage.Should().NotBeNull();
			createdImage.ImageURL.Should().Be("apple_new_side.jpg");
			createdImage.ProductID.Should().Be(10);
		}

		/// <summary>
		/// Verifies that an admin can update an existing product image (#100), returning NoContent on success.
		/// </summary>
		[Fact]
		public async Task UpdateProductImage_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedImage = new ProductImage
			{
				ImageID = 100, 
				ProductID = 10,
				ImageURL = "apple_strudel_updated.jpg",
				Product = new Product
				{
					ProductID = 10,
					Name = "Apple Strudel",
					Description = "Delicious apple pastry",
					Price = 5.99m,
					ImageURL = "apple.jpg",
					ProductImages = new List<ProductImage>()
				}
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/productimage/100", updatedImage);

			// ASSERT
			if (response.StatusCode != HttpStatusCode.NoContent)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				errorContent.Should().BeNullOrEmpty($"Expected successful product image update, but got errors: {errorContent}");
			}

			response.StatusCode.Should().Be(HttpStatusCode.NoContent, $"Expected 204 NoContent but got {response.StatusCode}");
		}

		
		/// <summary>
		/// Verifies that an admin can delete an existing product image (#101), returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteProductImage_AsAdmin_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/productimage/101");

			// ASSERT
			response.StatusCode.Should().BeOneOf(
			HttpStatusCode.NoContent,
			HttpStatusCode.NotFound
			);
		}
	}
}
