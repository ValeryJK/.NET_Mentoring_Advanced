using Asp.Versioning;
using Catalog.Persistence.Initialization;
using Catalog.Persistence.Initialization.Seed;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Mvc;

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

		public static IServiceCollection AddInfrastructure(this WebApplicationBuilder builder)
		{
			builder.Services.AddHttpContextAccessor();

			builder.Services.AddApiVersioning(options =>
			{
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ApiVersionReader = new UrlSegmentApiVersionReader();
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ReportApiVersions = true;
			});

			builder.Services
				.AddApiVersioning()
				.AddApiExplorer(options =>
				{

					options.GroupNameFormat = "'v'VVV";
					options.SubstituteApiVersionInUrl = true;
				});

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddFluentValidationAutoValidation(conf =>
			{
				conf.DisableDataAnnotationsValidation = true;
			});

			builder.Services.Configure<ApiBehaviorOptions>(options =>
					   options.SuppressModelStateInvalidFilter = true);

			ConfigureMapper();

			return builder.Services;
		}

		private static void ConfigureMapper()
		{
			TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);
		}
	}
}
