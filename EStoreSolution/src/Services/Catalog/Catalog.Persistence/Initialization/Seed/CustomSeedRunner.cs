using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Persistence.Initialization.Seed
{
	public class CustomSeedRunner : ICustomSeedRunner
	{
		private readonly ICustomSeeder[] _seeders;
		private readonly ILogger<CustomSeedRunner> _logger;

		public CustomSeedRunner(IServiceProvider serviceProvider, ILogger<CustomSeedRunner> logger)
		{
			_seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();
			_logger = logger;
		}
				
		public async Task RunSeeders(bool isDevelopment)
		{
			foreach (var seeder in _seeders.OrderBy(c => c.Order))
			{
				if (!seeder.IsDevelopmentData || (seeder.IsDevelopmentData && isDevelopment))
				{
					_logger.LogInformation("Running {Type}", seeder.GetType().FullName);
					await seeder.Initialize();
				}
			}
		}
	}
}
