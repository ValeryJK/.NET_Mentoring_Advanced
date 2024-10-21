using Cart.Application.Interfaces;
using Cart.Domain.Entities;
using Cart.Domain.Interfaces;

namespace Cart.Application.Services
{
	public class CartService : ICartService
	{
		private readonly ICartRepository _repository;

		public CartService(ICartRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<Domain.Entities.Cart>> GetAllCartsAsync()
		{
			return await _repository.GetAllCartsAsync();
		}

		public async Task<Domain.Entities.Cart?> GetCartAsync(Guid cartId)
		{
			return await _repository.GetCartAsync(cartId);
		}

		public async Task AddItemAsync(Guid cartId, CartItem item)
		{
			var cart = await _repository.GetCartAsync(cartId);

			if (cart is null)
			{
				cart = new Domain.Entities.Cart { Id = cartId, CartItems = new List<CartItem> { item } };
			}
			else
			{
				var existingItem = cart.CartItems.Find(i => i.Id == item.Id);
				if (existingItem is not null)
				{
					existingItem.Quantity += item.Quantity;
				}
				else
				{
					cart.CartItems.Add(item);
				}
			}

			await _repository.SaveCartAsync(cart);
		}

		public async Task RemoveItemAsync(Guid cartId, int itemId)
		{
			var cart = await _repository.GetCartAsync(cartId);
			if (cart is null) return;

			cart.CartItems.RemoveAll(i => i.Id == itemId);
			await _repository.SaveCartAsync(cart);
		}

		public async Task ClearCartAsync(Guid cartId)
		{
			var cart = await _repository.GetCartAsync(cartId);
			if (cart is null) return;

			cart.CartItems.Clear();
			await _repository.SaveCartAsync(cart);
		}
	}
}
