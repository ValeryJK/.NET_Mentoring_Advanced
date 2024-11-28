using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task DeleteCategoryWithProductsAsync(int categoryId);
    }
}
