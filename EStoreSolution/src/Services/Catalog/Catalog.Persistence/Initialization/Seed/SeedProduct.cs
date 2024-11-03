using Catalog.Persistence.Context;

namespace Catalog.Persistence.Initialization.Seed
{
	public class SeedProduct : ICustomSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly object _locker = new object();

		public bool IsDevelopmentData => true;
		public int Order => 2;

		public SeedProduct(ApplicationDbContext context)
		{
			_context = context;
		}

		public Task Initialize()
		{
			lock (_locker)
			{
				using (var transaction = _context.Database.BeginTransaction())
				{
					_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

					foreach (var product in SeedData.Products)
					{
						if (_context.Products.Any(p => p.Id == product.Id))
						{
							continue;
						}

						_context.Products.Add(product);
					}

					_context.SaveChanges();
					_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
					transaction.Commit();
				}
			}

			return Task.CompletedTask;
		}
	}
}
