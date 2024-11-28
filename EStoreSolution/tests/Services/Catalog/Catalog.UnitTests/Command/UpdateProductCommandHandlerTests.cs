using Catalog.Application.Features.Products.UpdateProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MassTransit;
using Moq;

namespace Catalog.UnitTests.Command
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> productRepositoryMock;
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;

        private readonly Mock<IPublishEndpoint> publishEndpoint;
        private readonly UpdateProductCommandHandler handler;

        public UpdateProductCommandHandlerTests()
        {
            this.productRepositoryMock = new Mock<IProductRepository>();
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.publishEndpoint = new Mock<IPublishEndpoint>();
            this.handler = new UpdateProductCommandHandler(this.productRepositoryMock.Object, this.categoryRepositoryMock.Object, this.publishEndpoint.Object);
        }

        [Fact]
        public async Task Handle_ProductExists_UpdatesProductAndReturnsResponse()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Image = "updated-image.png",
                Price = 99.99m,
                Amount = 10,
                CategoryId = 2
            };

            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Product",
                CategoryId = 1
            };

            var existingCategory = new Category { Id = 2, Name = "Category 2" };

            this.productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(existingProduct);
            this.categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(command.CategoryId)).ReturnsAsync(existingCategory);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(command.Name, result.Value.Name);
            Assert.Equal(command.Description, result.Value.Description);
            this.productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}
