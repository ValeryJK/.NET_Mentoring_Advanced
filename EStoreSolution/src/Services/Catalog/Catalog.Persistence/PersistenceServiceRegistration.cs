using Catalog.Domain.Interfaces;
using Catalog.Persistence.Context;
using Catalog.Persistence.Initialization;
using Catalog.Persistence.Initialization.Seed;
using Catalog.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("CatalogDb"));
            }
            else
            {
                var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        connectionString,
                        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IAsyncRepository<>))
                .AddClasses(classes => classes.AssignableTo(typeof(IAsyncRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(BaseRepository<>))
                .AddClasses(classes => classes.AssignableTo(typeof(BaseRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<IDatabaseInitializer, ApplicationDbInitializer>();
            services.AddScoped<IApplicationDbSeeder, ApplicationDbSeeder>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(ICustomSeeder))
                .AddClasses(classes => classes.AssignableTo(typeof(ICustomSeeder)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddScoped<ICustomSeedRunner, CustomSeedRunner>();

            return services;
        }
    }
}
