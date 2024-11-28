using System.Text;
using Catalog.Application.Features.Categories.CreateCategory;
using Catalog.IntegrationTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Catalog.IntegrationTests.Controllers
{
    public class CategoriesControllerIntegrationTests : BaseIntegrationTest
    {
        public CategoriesControllerIntegrationTests(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetAllCategories_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await this.httpClient.GetAsync("/api/v1/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content?.Headers?.ContentType?.ToString());
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedStatus()
        {
            // Arrange
            var newCategory = new CreateCategoryCommand
            {
                Name = "Test Category",
                Image = "test-image.png"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newCategory), Encoding.UTF8, "application/json");

            // Act
            var response = await this.httpClient.PostAsync("/api/v1/categories", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(responseContent);

            Assert.Equal(newCategory.Name, createdCategory?.Name);
        }
    }
}
