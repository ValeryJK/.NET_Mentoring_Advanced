namespace Catalog.Persistence.Initialization.Seed
{
	public interface IApplicationDbSeeder
	{
		Task SeedDatabase(bool isDevelopment);
	}
}
