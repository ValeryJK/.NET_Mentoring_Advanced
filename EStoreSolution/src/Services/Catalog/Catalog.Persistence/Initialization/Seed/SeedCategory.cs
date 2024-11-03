using Catalog.Persistence.Context;

namespace Catalog.Persistence.Initialization.Seed
{
	public class SeedCategory : ICustomSeeder
	{
		private readonly ApplicationDbContext _context;
		private static readonly object _locker = new object();

		public bool IsDevelopmentData => false;
		public int Order => 1;

		public SeedCategory(ApplicationDbContext context)
		{
			_context = context;
		}

		public Task Initialize()
		{
			lock (_locker)
			{
				using (var transaction = _context.Database.BeginTransaction())
				{
					_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");
					foreach (var category in SeedData.Categories)
					{
						if (!_context.Categories.Any(c => c.Name == category.Name))
						{
							_context.Categories.Add(category);
						}
					}

					_context.SaveChanges();
					_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");
					transaction.Commit();
				}
			}

			return Task.CompletedTask;
		}
	}
}
