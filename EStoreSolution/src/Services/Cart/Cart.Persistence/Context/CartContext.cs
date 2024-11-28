namespace Cart.Persistence.Context
{
    public class CartContext : ICartContext
    {
        public CartContext(IMongoClient mongoClient, string databaseName, string cartCollectionName)
        {
            var database = mongoClient.GetDatabase(databaseName);
            this.Carts = database.GetCollection<Domain.Entities.Cart>(cartCollectionName);
        }

        public IMongoCollection<Domain.Entities.Cart> Carts { get; }
    }
}
