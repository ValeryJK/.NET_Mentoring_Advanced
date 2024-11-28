using Catalog.Persistence.Context;

namespace Catalog.Persistence.Initialization.Seed
{
    public class SeedProduct : ICustomSeeder
    {
        private readonly ApplicationDbContext context;
        private readonly object locker = new object();

        public bool IsDevelopmentData => true;

        public int Order => 2;

        public SeedProduct(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task Initialize()
        {
            lock (this.locker)
            {
                using (var transaction = this.context.Database.BeginTransaction())
                {
                    this.context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

                    foreach (var product in SeedData.Products)
                    {
                        if (this.context.Products.Any(p => p.Id == product.Id))
                        {
                            continue;
                        }

                        this.context.Products.Add(product);
                    }

                    this.context.SaveChanges();
                    this.context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }
    }
}
