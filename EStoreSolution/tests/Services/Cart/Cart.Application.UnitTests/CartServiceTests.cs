using FluentAssertions;
using Cart.Application.Interfaces;
using Cart.Application.Services;
using Cart.Domain.Entities;
using Cart.Domain.Interfaces;
using Moq;

namespace Cart.Application.UnitTests
{
	public class CartServiceTests
	{
		private readonly Mock<ICartRepository> _mockRepository;
		private readonly ICartService _cartService;

		public CartServiceTests()
		{
			_mockRepository = new Mock<ICartRepository>();
			_cartService = new CartService(_mockRepository.Object);
		}

		[Fact]
		public async Task GetCartAsync_ShouldReturnCart_WhenCartExists()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var expectedCart = new Domain.Entities.Cart { Id = cartId, CartItems = new List<CartItem>() };
			_mockRepository.Setup(r => r.GetCartAsync(cartId)).ReturnsAsync(expectedCart);

			// Act
			var cart = await _cartService.GetCartAsync(cartId);

			// Assert
			cart.Should().NotBeNull();
			cart!.Id.Should().Be(cartId);
		}

		[Fact]
		public async Task AddItemAsync_ShouldAddItem_WhenCartDoesNotExist()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var newItem = new CartItem { Id = 1, Name = "Item1", Quantity = 2, Price = 10.0m };
			_mockRepository.Setup(r => r.GetCartAsync(cartId)).ReturnsAsync((Domain.Entities.Cart?)null);

			// Act
			await _cartService.AddItemAsync(cartId, newItem);

			// Assert
			_mockRepository.Verify(r => r.SaveCartAsync(It.Is<Domain.Entities.Cart>(c =>
				c.Id == cartId && c.CartItems.Contains(newItem))), Times.Once);
		}

		[Fact]
		public async Task AddItemAsync_ShouldIncreaseQuantity_WhenItemAlreadyExists()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var existingItem = new CartItem { Id = 1, Name = "Item1", Quantity = 2, Price = 10.0m };
			var cart = new Domain.Entities.Cart { Id = cartId, CartItems = new List<CartItem> { existingItem } };
			var newItem = new CartItem { Id = 1, Name = "Item1", Quantity = 3, Price = 10.0m };
			_mockRepository.Setup(r => r.GetCartAsync(cartId)).ReturnsAsync(cart);

			// Act
			await _cartService.AddItemAsync(cartId, newItem);

			// Assert
			_mockRepository.Verify(r => r.SaveCartAsync(It.Is<Domain.Entities.Cart>(c =>
				c.Id == cartId && c.CartItems[0].Quantity == 5)), Times.Once);
		}

		[Fact]
		public async Task RemoveItemAsync_ShouldRemoveItem_WhenItemExists()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var itemToRemove = new CartItem { Id = 1, Name = "Item1", Quantity = 2, Price = 10.0m };
			var cart = new Domain.Entities.Cart { Id = cartId, CartItems = new List<CartItem> { itemToRemove } };
			_mockRepository.Setup(r => r.GetCartAsync(cartId)).ReturnsAsync(cart);

			// Act
			await _cartService.RemoveItemAsync(cartId, itemToRemove.Id);

			// Assert
			_mockRepository.Verify(r => r.SaveCartAsync(It.Is<Domain.Entities.Cart>(c =>
				c.Id == cartId && c.CartItems.Count == 0)), Times.Once);
		}

		[Fact]
		public async Task ClearCartAsync_ShouldRemoveAllItems_WhenCartExists()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var cart = new Domain.Entities.Cart
			{
				Id = cartId,
				CartItems = new List<CartItem>
				{
					new CartItem { Id = 1, Name = "Item1", Quantity = 2, Price = 10.0m },
					new CartItem { Id = 2, Name = "Item2", Quantity = 1, Price = 20.0m }
				}
			};
			_mockRepository.Setup(r => r.GetCartAsync(cartId)).ReturnsAsync(cart);

			// Act
			await _cartService.ClearCartAsync(cartId);

			// Assert
			_mockRepository.Verify(r => r.SaveCartAsync(It.Is<Domain.Entities.Cart>(c =>
				c.Id == cartId && c.CartItems.Count == 0)), Times.Once);
		}
	}
}
