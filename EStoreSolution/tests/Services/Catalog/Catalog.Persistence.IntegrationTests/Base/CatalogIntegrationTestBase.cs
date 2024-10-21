using Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Persistence.IntegrationTests.Base
{
	public class CatalogIntegrationTestBase
	{
		protected readonly ApplicationDbContext CatalogContext;

		public CatalogIntegrationTestBase()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();

			bool useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");

			var serviceCollection = new ServiceCollection();
			if (useInMemoryDatabase)
			{
				serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryCatalogTestDb"));
			}
			else
			{
				serviceCollection.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(configuration.GetConnectionString("DatabaseSettings:ConnectionString")));
			}

			var serviceProvider = serviceCollection.BuildServiceProvider();
			var scope = serviceProvider.CreateScope();
			CatalogContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			if (!useInMemoryDatabase)
			{
				CatalogContext.Database.Migrate();
			}
		}

		protected void CleanDatabase()
		{
			CatalogContext.Categories.RemoveRange(CatalogContext.Categories);
			CatalogContext.Products.RemoveRange(CatalogContext.Products);
			CatalogContext.SaveChanges();
		}
	}
}
