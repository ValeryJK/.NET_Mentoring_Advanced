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

		public async Task<Domain.Entities.Cart?> GetCartAsync(Guid cartId)
		{
			using var cursor = await _carts.FindAsync(c => c.Id == cartId);
			return await cursor.FirstOrDefaultAsync();
		}

		public async Task SaveCartAsync(Domain.Entities.Cart cart)
		{
			await _carts.ReplaceOneAsync(
				c => c.Id == cart.Id,
				cart,
				new ReplaceOptions { IsUpsert = true }
			);
		}

		public async Task DeleteCartAsync(Guid cartId)
		{
			await _carts.DeleteOneAsync(c => c.Id == cartId);
		}
	}
}