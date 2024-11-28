using Catalog.Persistence.Initialization.Seed;
using Microsoft.Extensions.Logging;

namespace Catalog.Persistence.Initialization
{
    public class ApplicationDbSeeder : IApplicationDbSeeder
    {
        private readonly ICustomSeedRunner seederRunner;
        private readonly ILogger<ApplicationDbSeeder> logger;

        public ApplicationDbSeeder(ICustomSeedRunner seederRunner, ILogger<ApplicationDbSeeder> logger)
        {
            this.seederRunner = seederRunner;
            this.logger = logger;
        }

        public async Task SeedDatabase(bool isDevelopment)
        {
            this.logger.LogInformation("Running seed runners");
            await this.seederRunner.RunSeeders(isDevelopment);
        }
    }
}
