using Catalog.Application.Features.Categories.CreateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Mapster;
using Moq;

namespace Catalog.UnitTests.Command
{
	public class CreateCategoryCommandHandlerTests
	{
		private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
		private readonly CreateCategoryCommandHandler _handler;

		public CreateCategoryCommandHandlerTests()
		{
			_categoryRepositoryMock = new Mock<ICategoryRepository>();
			_handler = new CreateCategoryCommandHandler(_categoryRepositoryMock.Object);
		}

		[Fact]
		public async Task Handle_ValidCommand_ReturnsSuccessResult()
		{
			// Arrange
			var command = new CreateCategoryCommand { Name = "Test Category", Image = "test.jpg" };
			var category = command.Adapt<Category>();
			category.Id = 1;

			_categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>()))
				.ReturnsAsync(category);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(1, result.Value.Id);
			Assert.Equal("Test Category", result.Value.Name);
			_categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
		}
	}
}
