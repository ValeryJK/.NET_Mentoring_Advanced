using Catalog.Domain.Entities;
using Catalog.IntegrationTests.Base;
using FluentAssertions;

namespace Catalog.IntegrationTests.Repositories
{
    public class ProductRepositoryIntegrationTests : CatalogIntegrationTestBase
    {
        public ProductRepositoryIntegrationTests()
        {
            CleanDatabase();
        }

        [Fact]
        public async Task AddProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Image = "https://example.com/default-image.png",
                Description = "Description",
                Price = 10.99m,
                Amount = 5,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Test Category"
                }
            };

            // Act
            await CatalogContext.Products.AddAsync(product);
            await CatalogContext.SaveChangesAsync();

            // Assert
            var insertedProduct = await CatalogContext.Products.FindAsync(product.Id);
            insertedProduct.Should().NotBeNull();
            insertedProduct!.Name.Should().Be("Test Product");
        }

        [Fact]
        public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Existing Product",
                Image = "https://example.com/default-image.png",
                Description = "Description",
                Price = 15.99m,
                Amount = 10,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Test Category"
                }
            };
            await CatalogContext.Products.AddAsync(product);
            await CatalogContext.SaveChangesAsync();

            // Act
            var retrievedProduct = await CatalogContext.Products.FindAsync(product.Id);

            // Assert
            retrievedProduct.Should().NotBeNull();
            retrievedProduct!.Name.Should().Be("Existing Product");
        }

        [Fact]
        public async Task DeleteProduct_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Product to delete",
                Image = "https://example.com/default-image.png",
                Description = "Description",
                Price = 8.99m,
                Amount = 1,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Test Category"
                }
            };
            await CatalogContext.Products.AddAsync(product);
            await CatalogContext.SaveChangesAsync();

            // Act
            CatalogContext.Products.Remove(product);
            await CatalogContext.SaveChangesAsync();

            // Assert
            var deletedProduct = await CatalogContext.Products.FindAsync(product.Id);
            deletedProduct.Should().BeNull();
        }
    }
}