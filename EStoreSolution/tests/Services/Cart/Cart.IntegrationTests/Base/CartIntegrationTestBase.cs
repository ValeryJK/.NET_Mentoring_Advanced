using Cart.Persistence.Context;
using Mongo2Go;
using MongoDB.Driver;

namespace Cart.IntegrationTests.Base
{
	public class CartIntegrationTestBase : IDisposable
	{
		protected readonly ICartContext CartContext;
		private readonly MongoDbRunner _mongoRunner;
		private bool _disposed = false;

		public CartIntegrationTestBase()
		{
			_mongoRunner = MongoDbRunner.Start();

			var mongoClient = new MongoClient(_mongoRunner.ConnectionString);
			var mongoDatabaseName = "TestDatabase";
			var cartCollectionName = "Carts";

			CartContext = new CartContext(mongoClient, mongoDatabaseName, cartCollectionName);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				_mongoRunner?.Dispose();
			}

			_disposed = true;
		}
	}
}
