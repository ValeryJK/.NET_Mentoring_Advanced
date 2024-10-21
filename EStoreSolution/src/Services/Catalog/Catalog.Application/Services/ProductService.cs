using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _repository;

		public ProductService(IProductRepository repository)
		{
			_repository = repository;
		}

		public async Task AddAsync(Product product)
		{
			ArgumentNullException.ThrowIfNull(product);

			await _repository.AddAsync(product);
		}

		public async Task DeleteAsync(int id)
		{
			var product = await _repository.GetByIdAsync(id);
			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}

			await _repository.DeleteAsync(product);
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			return await _repository.ListAllAsync();
		}

		public async Task<Product> GetAsync(int id)
		{
			var product = await _repository.GetByIdAsync(id);
			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}

			return product;
		}

		public async Task UpdateAsync(Product product)
		{
			ArgumentNullException.ThrowIfNull(product);

			var existingProduct = await _repository.GetByIdAsync(product.Id);
			if (existingProduct == null)
			{
				throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
			}

			await _repository.UpdateAsync(product);
		}
	}
}
