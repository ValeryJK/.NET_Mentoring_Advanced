using FluentAssertions;
using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.Application.UnitTests
{
	public class ProductServiceTests
	{
		private readonly Mock<IProductRepository> _mockRepository;
		private readonly IProductService _productService;

		public ProductServiceTests()
		{
			_mockRepository = new Mock<IProductRepository>();
			_productService = new ProductService(_mockRepository.Object);
		}

		[Fact]
		public async Task AddAsync_ShouldAddProduct_WhenProductIsValid()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Test Product",
				Price = 100.0m,
				Amount = 10,
				CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				}
			};

			// Act
			await _productService.AddAsync(product);

			// Assert
			_mockRepository.Verify(r => r.AddAsync(product), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
		{
			// Arrange
			var productId = 1;
			var product = new Product
			{
				Id = productId,
				Name = "Test Product",
				CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				}
			};
			_mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

			// Act
			await _productService.DeleteAsync(productId);

			// Assert
			_mockRepository.Verify(r => r.DeleteAsync(product), Times.Once);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllProducts()
		{
			// Arrange
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product 1", CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				} },
				new Product { Id = 2, Name = "Product 2", CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				}}
			};
			_mockRepository.Setup(r => r.ListAllAsync()).ReturnsAsync(products);

			// Act
			var result = await _productService.GetAllAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
			result.Should().BeEquivalentTo(products);
		}

		[Fact]
		public async Task GetAsync_ShouldReturnProduct_WhenProductExists()
		{
			// Arrange
			var productId = 1;
			var product = new Product
			{
				Id = productId,
				Name = "Test Product",
				CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				}
			};
			_mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

			// Act
			var result = await _productService.GetAsync(productId);

			// Assert
			result.Should().NotBeNull();
			result!.Id.Should().Be(productId);
			result.Name.Should().Be("Test Product");
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
		{
			// Arrange
			var product = new Product { Id = 1, Name = "Updated Product", Price = 150.0m, Amount = 5, CategoryId = 1,
				Category = new Category
				{
					Id = 1,
					Name = "Test Category"
				}};
			_mockRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

			// Act
			await _productService.UpdateAsync(product);

			// Assert
			_mockRepository.Verify(r => r.UpdateAsync(product), Times.Once);
		}
	}
}