namespace Cart.Domain.Interfaces
{
	public interface ICartRepository
	{
		Task<Domain.Entities.Cart?> GetCartByIdAsync(string cartId);
		Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync();
		Task SaveCartAsync(Domain.Entities.Cart cart);
		Task DeleteCartAsync(string cartId);
		Task<Domain.Entities.CartItem?> GetItemByIdAsync(int Id);
	}
}
