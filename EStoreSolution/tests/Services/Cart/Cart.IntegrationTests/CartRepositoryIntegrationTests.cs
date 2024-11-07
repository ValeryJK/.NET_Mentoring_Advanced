using Cart.Domain.Entities;
using Cart.IntegrationTests.Base;
using FluentAssertions;
using MongoDB.Driver;

namespace Cart.IntegrationTests
{
	public class CartRepositoryIntegrationTests : CartIntegrationTestBase
	{
		[Fact]
		public async Task AddCart_ShouldAddCartToDatabase()
		{
			// Arrange
			var cart = new Domain.Entities.Cart
			{
				Id = Guid.NewGuid().ToString(),
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						Id = 1,
						Name = "Test Item",
						Price = 9.99m,
						Quantity = 1
					}
				}
			};

			// Act
			await CartContext.Carts.InsertOneAsync(cart);

			// Assert
			var filter = Builders<Domain.Entities.Cart>.Filter.Eq(c => c.Id, cart.Id);
			using var cursor = await CartContext.Carts.FindAsync(filter);
			var insertedCart = await cursor.FirstOrDefaultAsync();

			insertedCart.Should().NotBeNull();
			insertedCart.Id.Should().Be(cart.Id);
			insertedCart.CartItems.Should().HaveCount(1);
			insertedCart.CartItems[0].Name.Should().Be("Test Item");
		}

		[Fact]
		public async Task GetCart_ShouldReturnCart_WhenCartExists()
		{
			// Arrange
			var cart = new Domain.Entities.Cart
			{
				Id = Guid.NewGuid().ToString(),
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						Id = 1,
						Name = "Existing Item",
						Price = 19.99m,
						Quantity = 2
					}
				}
			};
			await CartContext.Carts.InsertOneAsync(cart);

			// Act
			var filter = Builders<Cart.Domain.Entities.Cart>.Filter.Eq(c => c.Id, cart.Id);
			using var cursor = await CartContext.Carts.FindAsync(filter);
			var retrievedCart = await cursor.FirstOrDefaultAsync();

			// Assert
			retrievedCart.Should().NotBeNull();
			retrievedCart.Id.Should().Be(cart.Id);
			retrievedCart.CartItems.Should().HaveCount(1);
			retrievedCart.CartItems[0].Name.Should().Be("Existing Item");
		}

		[Fact]
		public async Task DeleteCart_ShouldRemoveCartFromDatabase()
		{
			// Arrange
			var cart = new Domain.Entities.Cart
			{
				Id = Guid.NewGuid().ToString(),
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						Id = 1,
						Name = "To be deleted",
						Price = 5.99m,
						Quantity = 1
					}
				}
			};
			await CartContext.Carts.InsertOneAsync(cart);

			// Act
			await CartContext.Carts.DeleteOneAsync(c => c.Id == cart.Id);

			// Assert
			var filter = Builders<Cart.Domain.Entities.Cart>.Filter.Eq(c => c.Id, cart.Id);
			using var cursor = await CartContext.Carts.FindAsync(filter);
			var deletedCart = await cursor.FirstOrDefaultAsync();

			deletedCart.Should().BeNull();
		}
	}
}
