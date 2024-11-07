using Catalog.Domain.Common;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
	public interface IProductRepository : IAsyncRepository<Product>
	{
		Task<PagedResponse<Product>> GetProductsPagedAsync(int pageNumber, int pageSize, int? categoryId = null);
		Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId);
	}
}
