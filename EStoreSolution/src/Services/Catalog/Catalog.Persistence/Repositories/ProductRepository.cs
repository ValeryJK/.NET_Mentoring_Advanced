using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
	public class ProductRepository : BaseRepository<Product>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
