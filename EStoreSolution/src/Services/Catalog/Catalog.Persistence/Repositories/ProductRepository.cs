using Catalog.Domain.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<PagedResponse<Product>> GetProductsPagedAsync(int pageNumber, int pageSize, int? categoryId = null)
        {
            var query = this.dbContext.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Product>(data, pageNumber, pageSize, totalCount);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await this.dbContext.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }
    }
}