namespace Cart.Domain.Interfaces
{
	public interface ICartRepository
	{
		Task<Domain.Entities.Cart?> GetCartAsync(Guid cartId);
		Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync();
		Task SaveCartAsync(Domain.Entities.Cart cart);
		Task DeleteCartAsync(Guid cartId);
	}
}
