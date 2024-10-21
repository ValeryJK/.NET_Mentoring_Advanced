using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.Application.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _repository;

		public CategoryService(ICategoryRepository repository)
		{
			_repository = repository;
		}

		public async Task AddAsync(Category category)
		{
			ArgumentNullException.ThrowIfNull(category);

			await _repository.AddAsync(category);
		}

		public async Task DeleteAsync(int id)
		{
			var category = await _repository.GetByIdAsync(id);
			if (category == null)
			{
				throw new KeyNotFoundException($"Category with ID {id} not found.");
			}

			await _repository.DeleteAsync(category);
		}

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			return await _repository.ListAllAsync();
		}

		public async Task<Category> GetAsync(int id)
		{
			var category = await _repository.GetByIdAsync(id);
			if (category == null)
			{
				throw new KeyNotFoundException($"Category with ID {id} not found.");
			}

			return category;
		}

		public async Task UpdateAsync(Category category)
		{
			ArgumentNullException.ThrowIfNull(category);

			var existingCategory = await _repository.GetByIdAsync(category.Id);
			if (existingCategory == null)
			{
				throw new KeyNotFoundException($"Category with ID {category.Id} not found.");
			}

			await _repository.UpdateAsync(category);
		}
	}
}
