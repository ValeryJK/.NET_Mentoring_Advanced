using Asp.Versioning;
using Catalog.API.Extensions;
using Catalog.API.Resources;
using Catalog.Application.Features.Products.CreateProduct;
using Catalog.Application.Features.Products.DeleteProduct;
using Catalog.Application.Features.Products.GetProduct;
using Catalog.Application.Features.Products.GetProducts;
using Catalog.Application.Features.Products.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[ApiController]
	[Route("api/v{version:apiVersion}/products")]
	[ApiVersion("1.0")]
	public class ProductsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ProductsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Gets a paginated list of products, optionally filtered by category ID.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetPagedProducts(
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] int? id = null)
		{
			var query = new GetPagedProductsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				CategoryId = id
			};

			var result = await _mediator.Send(query);
			return result.ToHttpResponse();
		}

		/// <summary>
		/// Gets a specific product by ID.
		/// </summary>
		[HttpGet("{id}", Name = nameof(GetProductById))]
		public async Task<IActionResult> GetProductById(int id)
		{
			var query = new GetProductQuery { Id = id };
			var result = await _mediator.Send(query);
			return result.ToHttpResponseWithHateoas(this, HateoasLinksFactory.GetProductLinks());
		}

		/// <summary>
		/// Adds a new product.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
		{
			var result = await _mediator.Send(command);
			return result.ToCreatedHttpResponse();
		}

		/// <summary>
		/// Updates an existing product.
		/// </summary>
		[HttpPut("{id}", Name = nameof(UpdateProduct))]
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
		{
			command.Id = id;
			var result = await _mediator.Send(command);
			return result.ToHttpResponse();
		}

		/// <summary>
		/// Deletes a product by ID.
		/// </summary>
		[HttpDelete("{id}", Name = nameof(DeleteProduct))]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var command = new DeleteProductCommand { Id = id };
			var result = await _mediator.Send(command);
			return result.ToHttpResponse();
		}
	}
}
