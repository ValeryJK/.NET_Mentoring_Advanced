using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task DeleteCategoryWithProductsAsync(int categoryId)
        {
            var category = await this.dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is not null)
            {
                this.dbContext.Products.RemoveRange(category.Products);
                this.dbContext.Categories.Remove(category);
                await this.dbContext.SaveChangesAsync();
            }
        }
    }
}