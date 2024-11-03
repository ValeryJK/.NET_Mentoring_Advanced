using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[Route("products")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var product = await _productService.GetAsync(id);
			if (product == null)
				return NotFound();
			return Ok(product);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _productService.GetAllAsync();
			return Ok(products);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] Product product)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _productService.AddAsync(product);
			return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Product product)
		{
			if (id != product.Id)
				return BadRequest("ID mismatch");

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _productService.UpdateAsync(product);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _productService.DeleteAsync(id);
			return NoContent();
		}
	}
}
