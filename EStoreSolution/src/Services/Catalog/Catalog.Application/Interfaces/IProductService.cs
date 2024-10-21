using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces
{
	public interface IProductService
	{
		Task<Product> GetAsync(int id);
		Task<IEnumerable<Product>> GetAllAsync();
		Task AddAsync(Product product);
		Task UpdateAsync(Product product);
		Task DeleteAsync(int id);
	}
}
