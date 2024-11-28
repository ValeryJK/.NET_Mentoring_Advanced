using Catalog.Application.Features.Categories.CreateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Mapster;
using Moq;

namespace Catalog.UnitTests.Command
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;
        private readonly CreateCategoryCommandHandler handler;

        public CreateCategoryCommandHandlerTests()
        {
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.handler = new CreateCategoryCommandHandler(this.categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "Test Category", Image = "test.jpg" };
            var category = command.Adapt<Category>();
            category.Id = 1;

            this.categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Test Category", result.Value.Name);
            this.categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
        }
    }
}
