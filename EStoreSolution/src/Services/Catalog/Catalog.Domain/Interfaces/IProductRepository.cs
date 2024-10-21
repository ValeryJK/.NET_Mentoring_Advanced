using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
	public interface IProductRepository : IAsyncRepository<Product>
	{
	}
}
