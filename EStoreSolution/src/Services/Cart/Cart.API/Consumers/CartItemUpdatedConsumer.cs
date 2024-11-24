using Cart.Domain.Interfaces;
using Contracts.Messages;
using MassTransit;

namespace Cart.API.Consumers
{
	/// <summary>
	/// Consumer for handling updates to cart items.
	/// </summary>
	public class CartItemUpdatedConsumer : IConsumer<UpdateCartItemEvent>
	{
		private readonly ICartRepository _cartRepository;
		private readonly ILogger<CartItemUpdatedConsumer> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="CartItemUpdatedConsumer"/> class.
		/// </summary>
		/// <param name="cartRepository">The cart repository.</param>
		/// <param name="logger">The logger instance.</param>
		public CartItemUpdatedConsumer(ICartRepository cartRepository, ILogger<CartItemUpdatedConsumer> logger)
		{
			_cartRepository = cartRepository;
			_logger = logger;
		}

		/// <summary>
		/// Handles the consumption of an update event and confirms upon successful handling.
		/// </summary>
		/// <param name="context">The context of the consumed message.</param>
		public async Task Consume(ConsumeContext<UpdateCartItemEvent> context)
		{
			var updatedItem = context.Message;

			try
			{
				var carts = await _cartRepository.GetAllCartsAsync();
				var targetCart = carts.FirstOrDefault(cart => cart.CartItems.Exists(item => item.Id == updatedItem.Id));

				if (targetCart is null)
				{
					_logger.LogWarning("ItemId: {ItemId} not found in any cart.", updatedItem.Id);
					return;
				}

				var cartItem = targetCart.CartItems.First(item => item.Id == updatedItem.Id);
				cartItem.Name = updatedItem.Name;
				cartItem.Price = updatedItem.Price;

				await _cartRepository.SaveCartAsync(targetCart);
				_logger.LogInformation("Successfully updated ItemId: {ItemId} in CartId: {CartId}", updatedItem.Id, targetCart.Id);

				// Confirm message processing
				await context.ConsumeCompleted;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing ItemId: {ItemId}", updatedItem.Id);
			}
		}
	}
}