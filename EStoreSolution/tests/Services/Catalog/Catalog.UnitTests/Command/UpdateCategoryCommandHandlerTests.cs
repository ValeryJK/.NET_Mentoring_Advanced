using Catalog.Application.Features.Categories.UpdateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.UnitTests.Command
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;
        private readonly UpdateCategoryCommandHandler handler;

        public UpdateCategoryCommandHandlerTests()
        {
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.handler = new UpdateCategoryCommandHandler(this.categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CategoryExists_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateCategoryCommand { Id = 1, Name = "Updated Name", Image = "updated.jpg" };
            var category = new Category { Id = 1, Name = "Old Name", Image = "old.jpg" };

            this.categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(category);
            this.categoryRepositoryMock.Setup(repo => repo.UpdateAsync(category))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Name", category.Name);
            Assert.Equal("updated.jpg", category.Image);
            this.categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }
    }
}
