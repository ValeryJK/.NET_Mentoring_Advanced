using System.Reflection;
using Cart.Persistence.Context;
using Cart.Persistence.Initialization;
using Microsoft.OpenApi.Models;

namespace Cart.API.Infrastructure
{
    /// <summary>
    /// Provides bootstrap methods for the application.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// AddSwaggerGenWithAuth.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "Cart API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                o.IncludeXmlComments(xmlPath);

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

        /// <summary>
        /// Adds custom health checks to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbConnectionString = configuration["DatabaseSettings:ConnectionString"]
                ?? throw new InvalidOperationException("The MongoDB connection string is not configured.");

            services.AddHealthChecks().AddMongoDb(mongoDbConnectionString, name: "MongoDB");
            return services;
        }

        /// <summary>
        /// Seeds the database with initial data.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        public static void SeedDatabase(this WebApplication app)
        {
            if (!app.Configuration.GetValue<bool>("DatabaseSettings:SeedDatabase"))
            {
                return;
            }

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
