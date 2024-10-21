using FluentAssertions;
using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.Application.UnitTests
{
	public class CategoryServiceTests
	{
		private readonly Mock<ICategoryRepository> _mockRepository;
		private readonly ICategoryService _categoryService;

		public CategoryServiceTests()
		{
			_mockRepository = new Mock<ICategoryRepository>();
			_categoryService = new CategoryService(_mockRepository.Object);
		}

		[Fact]
		public async Task AddAsync_ShouldAddCategory_WhenCategoryIsValid()
		{
			// Arrange
			var category = new Category { Id = 1, Name = "Test Category" };

			// Act
			await _categoryService.AddAsync(category);

			// Assert
			_mockRepository.Verify(r => r.AddAsync(category), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteCategory_WhenCategoryExists()
		{
			// Arrange
			var categoryId = 1;
			var category = new Category { Id = categoryId, Name = "Test Category" };
			_mockRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

			// Act
			await _categoryService.DeleteAsync(categoryId);

			// Assert
			_mockRepository.Verify(r => r.DeleteAsync(category), Times.Once);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllCategories()
		{
			// Arrange
			var categories = new List<Category>
			{
				new Category { Id = 1, Name = "Category 1" },
				new Category { Id = 2, Name = "Category 2" }
			};
			_mockRepository.Setup(r => r.ListAllAsync()).ReturnsAsync(categories);

			// Act
			var result = await _categoryService.GetAllAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
			result.Should().BeEquivalentTo(categories);
		}

		[Fact]
		public async Task GetAsync_ShouldReturnCategory_WhenCategoryExists()
		{
			// Arrange
			var categoryId = 1;
			var category = new Category { Id = categoryId, Name = "Test Category" };
			_mockRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

			// Act
			var result = await _categoryService.GetAsync(categoryId);

			// Assert
			result.Should().NotBeNull();
			result!.Id.Should().Be(categoryId);
			result.Name.Should().Be("Test Category");
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateCategory_WhenCategoryExists()
		{
			// Arrange
			var category = new Category { Id = 1, Name = "Updated Category" };
			_mockRepository.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

			// Act
			await _categoryService.UpdateAsync(category);

			// Assert
			_mockRepository.Verify(r => r.UpdateAsync(category), Times.Once);
		}
	}
}
