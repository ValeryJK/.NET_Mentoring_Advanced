using Cart.Domain.Common;
using Cart.Domain.Interfaces;
using Cart.Persistence.Context;
using Cart.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cart.Persistence
{
	public static class PersistenceServiceRegistration
	{
		public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

			services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings").Bind);

			services.AddSingleton(sp =>
			{
				var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
				return new MongoClient(settings.ConnectionString);
			});

			services.AddSingleton(sp =>
			{
				var client = sp.GetRequiredService<MongoClient>();
				var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
				return client.GetDatabase(settings.DatabaseName);
			});

			services.AddScoped<ICartContext, CartContext>(sp =>
			{
				var client = sp.GetRequiredService<MongoClient>();
				var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
				return new CartContext(client, settings.DatabaseName, settings.CartName);
			});

			services.Scan(scan => scan
				.FromAssembliesOf(typeof(CartRepository))
				.AddClasses(classes => classes.AssignableTo<ICartRepository>())
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			return services;
		}
	}
}
