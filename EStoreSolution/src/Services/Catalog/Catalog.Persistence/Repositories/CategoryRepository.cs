using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
	public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
	{
		public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}

		public async Task DeleteCategoryWithProductsAsync(int categoryId)
		{
			var category = await _dbContext.Categories
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == categoryId);

			if (category is not null)
			{
				_dbContext.Products.RemoveRange(category.Products);
				_dbContext.Categories.Remove(category);
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}