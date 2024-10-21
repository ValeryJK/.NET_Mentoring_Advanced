using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces
{
	public interface ICategoryService
	{
		Task<Category> GetAsync(int id);
		Task<IEnumerable<Category>> GetAllAsync();
		Task AddAsync(Category category);
		Task UpdateAsync(Category category);
		Task DeleteAsync(int id);
	}
}
