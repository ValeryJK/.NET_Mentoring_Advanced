using Asp.Versioning;
using Catalog.API.Extensions;
using Catalog.API.Resources;
using Catalog.Application.Features.Categories.CreateCategory;
using Catalog.Application.Features.Categories.DeleteCategory;
using Catalog.Application.Features.Categories.GetCategories;
using Catalog.Application.Features.Categories.GetCategory;
using Catalog.Application.Features.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator mediator;

        public CategoriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets a list of all categories.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Authorize(Policy = "ReadAccessPolicy")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await this.mediator.Send(new GetCategoriesQuery());
            return result.ToHttpResponse();
        }

        /// <summary>
        /// Gets a specific category by ID.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet("{id}", Name = nameof(GetCategoryById))]
        [Authorize(Policy = "ReadAccessPolicy")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var query = new GetCategoryQuery { Id = id };
            var result = await this.mediator.Send(query);

            return result.ToHttpResponseWithHateoas(this, HateoasLinksFactory.GetCategoryLinks());
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        [Authorize(Policy = "CreateAccessPolicy")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var result = await this.mediator.Send(command);
            return result.ToCreatedHttpResponse();
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("{id}", Name = nameof(UpdateCategory))]
        [Authorize(Policy = "UpdateAccessPolicy")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await this.mediator.Send(command);
            return result.ToHttpResponse();
        }

        /// <summary>
        /// Deletes category and its related products.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpDelete("{id}", Name = nameof(DeleteCategory))]
        [Authorize(Policy = "DeleteAccessPolicy")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteCategoryCommand { Id = id };
            var result = await this.mediator.Send(command);
            return result.ToHttpResponse();
        }
    }
}
