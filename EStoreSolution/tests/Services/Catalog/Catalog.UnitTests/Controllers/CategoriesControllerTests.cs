using Catalog.API.Controllers;
using Catalog.Application.Features.Categories.GetCategories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Catalog.UnitTests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly CategoriesController controller;
        private readonly Mock<IMediator> mediatorMock;

        public CategoriesControllerTests()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.controller = new CategoriesController(this.mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOkResult_WithListOfCategories()
        {
            // Arrange
            var categories = new List<GetCategoriesQueryResponse>
        {
            new GetCategoriesQueryResponse { Id = 1, Name = "Electronics" },
            new GetCategoriesQueryResponse { Id = 2, Name = "Books" }
        };

            var result = Result.Ok<IEnumerable<GetCategoriesQueryResponse>>(categories);

            this.mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await this.controller.GetAllCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GetCategoriesQueryResponse>>(okResult.Value);
            Assert.Equal(2, ((List<GetCategoriesQueryResponse>)returnValue).Count);
        }
    }
}
