using Cart.Domain.Entities;
using Cart.Domain.Interfaces;

namespace Cart.Persistence.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly IMongoCollection<Domain.Entities.Cart> _carts;

		public CartRepository(IMongoDatabase database)
		{
			_carts = database.GetCollection<Domain.Entities.Cart>("carts");
		}

		public async Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync()
		{
			using var cursor = await _carts.FindAsync(_ => true);
			return await cursor.ToListAsync();
		}

		public async Task<Domain.Entities.Cart?> GetCartByIdAsync(string cartId)
		{
			var filter = Builders<Domain.Entities.Cart>.Filter.Eq(x => x.Id, cartId);

			var cart = await _carts.FindAsync(filter);
			return await cart.FirstOrDefaultAsync();
		}

		public async Task SaveCartAsync(Domain.Entities.Cart cart)
		{
			await _carts.ReplaceOneAsync(c => c.Id == cart.Id, cart, new ReplaceOptions { IsUpsert = true });
		}

		public async Task DeleteCartAsync(string cartId)
		{
			await _carts.DeleteOneAsync(c => c.Id == cartId);
		}

		public async Task<CartItem?> GetItemByIdAsync(int Id)
		{
			using var cursor = await _carts.FindAsync(_ => true);
			var carts = await cursor.ToListAsync();

			return carts.SelectMany(cart => cart.CartItems).FirstOrDefault(item => item.Id == Id);
		}
	}
}