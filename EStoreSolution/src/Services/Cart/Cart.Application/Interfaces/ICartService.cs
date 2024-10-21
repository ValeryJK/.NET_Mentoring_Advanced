using Cart.Domain.Entities;

namespace Cart.Application.Interfaces
{
	public interface ICartService
	{
		Task<Domain.Entities.Cart?> GetCartAsync(Guid cartId);
		Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync();
		Task AddItemAsync(Guid cartId, CartItem item);
		Task RemoveItemAsync(Guid cartId, int itemId);
		Task ClearCartAsync(Guid cartId);
	}
}
