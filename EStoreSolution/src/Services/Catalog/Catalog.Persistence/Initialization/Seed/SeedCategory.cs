using Catalog.Persistence.Context;

namespace Catalog.Persistence.Initialization.Seed
{
    public class SeedCategory : ICustomSeeder
    {
        private readonly ApplicationDbContext context;
        private static readonly object Locker = new object();

        public bool IsDevelopmentData => false;

        public int Order => 1;

        public SeedCategory(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task Initialize()
        {
            lock (Locker)
            {
                using (var transaction = this.context.Database.BeginTransaction())
                {
                    this.context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");
                    foreach (var category in SeedData.Categories)
                    {
                        if (!this.context.Categories.Any(c => c.Name == category.Name))
                        {
                            this.context.Categories.Add(category);
                        }
                    }

                    this.context.SaveChanges();
                    this.context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }
    }
}
