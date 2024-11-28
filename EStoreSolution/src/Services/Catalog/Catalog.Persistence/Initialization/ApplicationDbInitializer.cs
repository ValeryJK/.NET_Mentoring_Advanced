using Catalog.Persistence.Context;
using Microsoft.Extensions.Logging;

namespace Catalog.Persistence.Initialization
{
    public class ApplicationDbInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ApplicationDbInitializer> logger;

        public ApplicationDbInitializer(ApplicationDbContext dbContext, ILogger<ApplicationDbInitializer> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            if (this.dbContext.Database.GetMigrations().Any() &&
                (await this.dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                this.logger.LogInformation("Applying migrations ...");
                await this.dbContext.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
