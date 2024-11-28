using Cart.Domain.Entities;
using Cart.Domain.Interfaces;

namespace Cart.Persistence.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Domain.Entities.Cart> carts;

        public CartRepository(IMongoDatabase database)
        {
            this.carts = database.GetCollection<Domain.Entities.Cart>("carts");
        }

        public async Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync()
        {
            using var cursor = await this.carts.FindAsync(_ => true);
            return await cursor.ToListAsync();
        }

        public async Task<Domain.Entities.Cart?> GetCartByIdAsync(string cartId)
        {
            var filter = Builders<Domain.Entities.Cart>.Filter.Eq(x => x.Id, cartId);

            var cart = await this.carts.FindAsync(filter);
            return await cart.FirstOrDefaultAsync();
        }

        public async Task SaveCartAsync(Domain.Entities.Cart cart)
        {
            await this.carts.ReplaceOneAsync(c => c.Id == cart.Id, cart, new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteCartAsync(string cartId)
        {
            await this.carts.DeleteOneAsync(c => c.Id == cartId);
        }

        public async Task<CartItem?> GetItemByIdAsync(int id)
        {
            using var cursor = await this.carts.FindAsync(_ => true);
            var cartsData = await cursor.ToListAsync();

            return cartsData.SelectMany(cart => cart.CartItems).FirstOrDefault(item => item.Id == id);
        }
    }
}