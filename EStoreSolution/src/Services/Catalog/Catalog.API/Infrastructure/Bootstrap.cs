using Asp.Versioning;
using Catalog.Persistence.Initialization;
using Catalog.Persistence.Initialization.Seed;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Catalog.API.Infrastructure
{
    public static class Bootstrap
    {
        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(configuration["Authentication:AuthorizationUrl"]
                                ?? throw new InvalidOperationException("Authentication:AuthorizationUrl configuration is missing.")),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID scope" },
                                { "profile", "Profile scope" }
                            }
                        }
                    }
                });

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Keycloak"
                            },
                            In = ParameterLocation.Header,
                            Name = "Authorization",
                            Scheme = "Bearer"
                        },
                        new List<string> { "openid", "profile" }
                    }
                };

                o.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }

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
