using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Persistence.Initialization.Seed
{
    public class CustomSeedRunner : ICustomSeedRunner
    {
        private readonly ICustomSeeder[] seeders;
        private readonly ILogger<CustomSeedRunner> logger;

        public CustomSeedRunner(IServiceProvider serviceProvider, ILogger<CustomSeedRunner> logger)
        {
            this.seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();
            this.logger = logger;
        }

        public async Task RunSeeders(bool isDevelopment)
        {
            foreach (var seeder in this.seeders.OrderBy(c => c.Order))
            {
                if (!seeder.IsDevelopmentData || (seeder.IsDevelopmentData && isDevelopment))
                {
                    this.logger.LogInformation("Running {Type}", seeder.GetType().FullName);
                    await seeder.Initialize();
                }
            }
        }
    }
}
