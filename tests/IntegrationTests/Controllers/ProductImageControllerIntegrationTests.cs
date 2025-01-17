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
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/productimage");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var images = await response.Content.ReadFromJsonAsync<List<ProductImage>>();
			images.Should().NotBeNull();
			images.Should().HaveCount(2);
		}

		/// <summary>
		/// Verifies fetching a product image by a known ID returns OK for admin.
		/// </summary>
		[Fact]
		public async Task GetProductImageById_AsAdmin_ReturnsOkIfExists()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/productimage/100");
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
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/productimage/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Verifies that an admin can create a new product image successfully.
		/// </summary>
		[Fact]
		public async Task CreateProductImage_AsAdmin_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var newImage = new ProductImage
			{
				ImageID = 200,
				ProductID = 10,
				ImageURL = "apple_new_side.jpg"
			};

			var response = await _client.PostAsJsonAsync("/api/productimage", newImage);
			response.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		/// <summary>
		/// Verifies that an admin can update an existing product image successfully.
		/// </summary>
		[Fact]
		public async Task UpdateProductImage_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var updatedImage = new ProductImage
			{
				ImageID = 100,
				ProductID = 10,
				ImageURL = "apple_strudel_updated.jpg"
			};

			var response = await _client.PutAsJsonAsync("/api/productimage/100", updatedImage);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Ensures a BadRequest is returned if the image ID in the route does not match the body.
		/// </summary>
		[Fact]
		public async Task UpdateProductImage_IdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var image = new ProductImage
			{
				ImageID = 100,
				ProductID = 10
			};

			var response = await _client.PutAsJsonAsync("/api/productimage/9999", image);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Verifies that an admin can delete an existing product image, returning NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteProductImage_AsAdmin_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client, "admin", "adminPass");
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/productimage/101");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}
	}
}
