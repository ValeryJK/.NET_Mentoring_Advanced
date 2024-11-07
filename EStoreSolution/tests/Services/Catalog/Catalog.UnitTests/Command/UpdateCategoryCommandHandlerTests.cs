using Catalog.Application.Features.Categories.UpdateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.UnitTests.Command
{
	public class UpdateCategoryCommandHandlerTests
	{
		private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
		private readonly UpdateCategoryCommandHandler _handler;

		public UpdateCategoryCommandHandlerTests()
		{
			_categoryRepositoryMock = new Mock<ICategoryRepository>();
			_handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);
		}

		[Fact]
		public async Task Handle_CategoryExists_ReturnsSuccessResult()
		{
			// Arrange
			var command = new UpdateCategoryCommand { Id = 1, Name = "Updated Name", Image = "updated.jpg" };
			var category = new Category { Id = 1, Name = "Old Name", Image = "old.jpg" };

			_categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
				.ReturnsAsync(category);
			_categoryRepositoryMock.Setup(repo => repo.UpdateAsync(category))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal("Updated Name", category.Name);
			Assert.Equal("updated.jpg", category.Image);
			_categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Once);
		}
	}
}
