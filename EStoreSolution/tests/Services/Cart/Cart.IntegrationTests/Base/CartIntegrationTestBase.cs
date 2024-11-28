using Cart.Persistence.Context;
using Mongo2Go;
using MongoDB.Driver;

namespace Cart.IntegrationTests.Base
{
    public class CartIntegrationTestBase : IDisposable
    {
        protected readonly ICartContext CartContext;
        private readonly MongoDbRunner mongoRunner;
        private bool disposed = false;

        public CartIntegrationTestBase()
        {
            this.mongoRunner = MongoDbRunner.Start();

            var mongoClient = new MongoClient(this.mongoRunner.ConnectionString);
            var mongoDatabaseName = "TestDatabase";
            var cartCollectionName = "Carts";

            this.CartContext = new CartContext(mongoClient, mongoDatabaseName, cartCollectionName);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.mongoRunner?.Dispose();
            }

            this.disposed = true;
        }
    }
}
