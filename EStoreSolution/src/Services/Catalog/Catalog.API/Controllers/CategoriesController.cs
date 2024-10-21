using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[Route("categories")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoriesController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var category = await _categoryService.GetAsync(id);
			if (category == null)
				return NotFound();
			return Ok(category);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var categories = await _categoryService.GetAllAsync();
			return Ok(categories);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] Category category)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _categoryService.AddAsync(category);
			return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Category category)
		{
			if (id != category.Id)
				return BadRequest("ID mismatch");

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _categoryService.UpdateAsync(category);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _categoryService.DeleteAsync(id);
			return NoContent();
		}
	}
}
