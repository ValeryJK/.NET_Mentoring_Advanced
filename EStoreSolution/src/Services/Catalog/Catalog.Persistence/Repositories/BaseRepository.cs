using Catalog.Domain.Common;
using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T>
        where T : BaseEntity
    {
        protected readonly ApplicationDbContext dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await this.dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await this.dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await this.dbContext.Set<T>().AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            this.dbContext.Set<T>().Remove(entity);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
