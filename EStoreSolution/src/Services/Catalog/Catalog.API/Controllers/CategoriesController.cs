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
		private readonly IMediator _mediator;

		public CategoriesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Gets a list of all categories.
		/// </summary>
		[HttpGet]
		[Authorize(Policy = "ReadAccessPolicy")]
		public async Task<IActionResult> GetAllCategories()
		{
			var result = await _mediator.Send(new GetCategoriesQuery());
			return result.ToHttpResponse();
		}

		/// <summary>
		/// Gets a specific category by ID.
		/// </summary>
		[HttpGet("{id}", Name = nameof(GetCategoryById))]
		[Authorize(Policy = "ReadAccessPolicy")]
		public async Task<IActionResult> GetCategoryById(int id)
		{
			var query = new GetCategoryQuery { Id = id };
			var result = await _mediator.Send(query);

			return result.ToHttpResponseWithHateoas(this, HateoasLinksFactory.GetCategoryLinks());
		}

		/// <summary>
		/// Adds a new category.
		/// </summary>
		[HttpPost]
		[Authorize(Policy = "CreateAccessPolicy")]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
		{
			var result = await _mediator.Send(command);
			return result.ToCreatedHttpResponse();
		}

		/// <summary>
		/// Updates an existing category.
		/// </summary>
		[HttpPut("{id}", Name = nameof(UpdateCategory))]
		[Authorize(Policy = "UpdateAccessPolicy")]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
		{
			command.Id = id;
			var result = await _mediator.Send(command);
			return result.ToHttpResponse();
		}

		/// <summary>
		/// Deletes category and its related products.
		/// </summary>
		[HttpDelete("{id}", Name = nameof(DeleteCategory))]
		[Authorize(Policy = "DeleteAccessPolicy")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var command = new DeleteCategoryCommand { Id = id };
			var result = await _mediator.Send(command);
			return result.ToHttpResponse();
		}
	}
}
