using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using DataAccess.Services;
using StrudelShop.DataAccess.DataAccess;
using DataAccess.Models;

namespace UnitTests.Services
{
	public class ProductImageServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<ProductImage>> _productImageDbSetMock;
		private readonly ProductImageService _service;

		public ProductImageServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_productImageDbSetMock = new Mock<DbSet<ProductImage>>();
			_dbContextMock.Setup(db => db.ProductImages).Returns(_productImageDbSetMock.Object);

			_service = new ProductImageService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests that GetProductImageByIdAsync returns the image when found.
		/// </summary>
		[Fact]
		public async Task GetProductImageByIdAsync_WhenFound_ReturnsImage()
		{
			// Arrange
			var testImage = new ProductImage { ImageID = 1, ProductID = 10 };
			_dbContextMock.Setup(db => db.ProductImages.FindAsync(1)).ReturnsAsync(testImage);

			// Act
			var result = await _service.GetProductImageByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.ImageID.Should().Be(1);
		}

		/// <summary>
		/// Tests that GetProductImageByIdAsync returns null when the image is not found.
		/// </summary>
		[Fact]
		public async Task GetProductImageByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock.Setup(db => db.ProductImages.FindAsync(999)).ReturnsAsync((ProductImage)null);

			// Act
			var result = await _service.GetProductImageByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllProductImagesAsync returns all images in the DbSet.
		/// </summary>
		[Fact]
		public async Task GetAllProductImagesAsync_ReturnsAllImages()
		{
			// Arrange
			var imagesData = new List<ProductImage>
			{
				new ProductImage { ImageID = 1, ProductID = 10 },
				new ProductImage { ImageID = 2, ProductID = 15 }
			}.AsQueryable();

			_productImageDbSetMock.As<IQueryable<ProductImage>>().Setup(m => m.Provider).Returns(imagesData.Provider);
			_productImageDbSetMock.As<IQueryable<ProductImage>>().Setup(m => m.Expression).Returns(imagesData.Expression);
			_productImageDbSetMock.As<IQueryable<ProductImage>>().Setup(m => m.ElementType).Returns(imagesData.ElementType);
			_productImageDbSetMock.As<IQueryable<ProductImage>>().Setup(m => m.GetEnumerator()).Returns(imagesData.GetEnumerator());

			// Act
			var result = await _service.GetAllProductImagesAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that CreateProductImageAsync adds and saves a new product image.
		/// </summary>
		[Fact]
		public async Task CreateProductImageAsync_AddsAndSaves()
		{
			// Arrange
			var newImage = new ProductImage { ImageID = 3, ProductID = 20 };

			// Act
			await _service.CreateProductImageAsync(newImage);

			// Assert
			_dbContextMock.Verify(db => db.ProductImages.AddAsync(newImage, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that UpdateProductImageAsync updates the image and saves.
		/// </summary>
		[Fact]
		public async Task UpdateProductImageAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingImage = new ProductImage { ImageID = 5 };

			// Act
			await _service.UpdateProductImageAsync(existingImage);

			// Assert
			_dbContextMock.Verify(db => db.ProductImages.Update(existingImage), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteProductImageAsync removes and saves the image if found.
		/// </summary>
		[Fact]
		public async Task DeleteProductImageAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingImage = new ProductImage { ImageID = 10 };
			_dbContextMock.Setup(db => db.ProductImages.FindAsync(10)).ReturnsAsync(existingImage);

			// Act
			await _service.DeleteProductImageAsync(10);

			// Assert
			_dbContextMock.Verify(db => db.ProductImages.Remove(existingImage), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteProductImageAsync does nothing if the image is not found.
		/// </summary>
		[Fact]
		public async Task DeleteProductImageAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.ProductImages.FindAsync(999)).ReturnsAsync((ProductImage)null);

			// Act
			await _service.DeleteProductImageAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.ProductImages.Remove(It.IsAny<ProductImage>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}
	}
}
