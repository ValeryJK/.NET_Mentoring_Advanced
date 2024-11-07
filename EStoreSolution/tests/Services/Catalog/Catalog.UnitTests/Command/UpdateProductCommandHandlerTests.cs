using Catalog.Application.Features.Products.UpdateProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.UnitTests.Command
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
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

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(existingProduct);
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(command.CategoryId)).ReturnsAsync(existingCategory);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(command.Name, result.Value.Name);
            Assert.Equal(command.Description, result.Value.Description);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}
