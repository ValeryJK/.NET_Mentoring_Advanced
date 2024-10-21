using Catalog.Persistence.Initialization.Seed;
using Catalog.Persistence.Initialization;

namespace Catalog.API.Infrastructure
{
	public static class Bootstrap
	{
		public static IServiceCollection AddSqlServerHealthCheck(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration["DatabaseSettings:ConnectionString"];

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("The connection string for the database is not configured.");
			}

			services.AddHealthChecks().AddSqlServer(connectionString, name: "SqlServer");
			return services;
		}
		public static async Task MigrateDatabase(this WebApplication app)
		{
			using (var scope = app.Services.CreateScope())
			{
				var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
				await dbInitializer.Initialize(CancellationToken.None);
			}
		}

		public static async Task ConfigureDatabaseAsync(this WebApplication app)
		{
			var configuration = app.Configuration;
			var migrateDatabase = configuration.GetValue<bool>("DatabaseSettings:MigrateDatabase");
			var seedDatabase = configuration.GetValue<bool>("DatabaseSettings:SeedDatabase");
			var isDevelopmentOrIt = app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test");

			if (migrateDatabase)
			{
				await app.MigrateDatabase();
			}

			if (seedDatabase)
			{
				await app.InitializeDatabase(isDevelopmentOrIt);
			}
		}

		public static async Task InitializeDatabase(this WebApplication app, bool isDevelopment)
		{
			using (var scope = app.Services.CreateScope())
			{
				var seeder = scope.ServiceProvider.GetRequiredService<IApplicationDbSeeder>();
				await seeder.SeedDatabase(isDevelopment);
			}
		}
	}
}
