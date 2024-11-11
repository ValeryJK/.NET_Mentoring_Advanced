using Cart.Domain.Interfaces;
using Cart.Persistence.Context;
using Cart.Persistence.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;

namespace Cart.IntegrationTests.Base
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		private MongoDbRunner _mongoRunner;
		public IMongoDatabase Database { get; private set; }

		public CustomWebApplicationFactory()
		{
			_mongoRunner = MongoDbRunner.Start();
			var client = new MongoClient(_mongoRunner.ConnectionString);
			Database = client.GetDatabase("TestDatabase");
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Test");			

			builder.ConfigureServices(services =>
			{
				services.RemoveAll(typeof(ICartContext));
				services.RemoveAll(typeof(ICartRepository));
				services.AddSingleton<IMongoDatabase>(sp => Database);

				services.AddSingleton<ICartContext>(sp =>
				{
					var configuration = sp.GetRequiredService<IConfiguration>();
					var mongoDbConnectionString = configuration["DatabaseSettings:ConnectionString"]
							?? throw new InvalidOperationException("The MongoDB connection string is not configured.");
					var client = new MongoClient(mongoDbConnectionString);
					var databaseName = "TestDatabase";
					var collectionName = "Carts";

					return new CartContext(client, databaseName, collectionName);
				});

				services.AddScoped<ICartRepository, CartRepository>();
			});
		}

		public void Initialize()
		{
			_mongoRunner = MongoDbRunner.Start();
			var client = new MongoClient(_mongoRunner.ConnectionString);
			Database = client.GetDatabase("TestDatabase");
		}

		public new void Dispose()
		{
			_mongoRunner.Dispose();
		}
	}
}
