using Cart.Persistence.Context;
using Cart.Persistence.Initialization;

namespace Cart.API.Infrastructure
{
	public static class Bootstrap
	{
		public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
		{
			var mongoDbConnectionString = configuration["DatabaseSettings:ConnectionString"]
				?? throw new InvalidOperationException("The MongoDB connection string is not configured.");

			services.AddHealthChecks().AddMongoDb(mongoDbConnectionString, name: "MongoDB");
			return services;
		}

		public static void SeedDatabase(this WebApplication app)
		{
			if (!app.Configuration.GetValue<bool>("DatabaseSettings:SeedDatabase")) return;

			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;
			var logger = services.GetRequiredService<ILogger<Program>>(); 

			try
			{
				var cartContext = services.GetRequiredService<ICartContext>();
				CartContextSeed.SeedData(cartContext.Carts);
				logger.LogInformation("Data seeded successfully.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred during data seeding.");
			}
		}
	}
}
