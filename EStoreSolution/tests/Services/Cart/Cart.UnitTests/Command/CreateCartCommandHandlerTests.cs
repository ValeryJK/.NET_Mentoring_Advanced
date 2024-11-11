using Cart.Application.Features.CreateCart;
using Cart.Domain.Entities;
using Cart.Domain.Interfaces;
using Moq;

namespace Cart.UnitTests.Command
{
	public class CreateCartCommandHandlerTests
	{
		private readonly CreateCartCommandHandler _handler;
		private readonly Mock<ICartRepository> _cartRepositoryMock;

		public CreateCartCommandHandlerTests()
		{
			_cartRepositoryMock = new Mock<ICartRepository>();
			_handler = new CreateCartCommandHandler(_cartRepositoryMock.Object);
		}

		[Fact]
		public async Task Handle_AddsItemsToExistingCart_WhenCartExists()
		{
			// Arrange
			var existingCart = new Cart.Domain.Entities.Cart
			{
				Id = "existing-cart-id",
				CartItems = new List<CartItem>()
			};
			var command = new CreateCartCommand
			{
				CartId = "existing-cart-id",
				InitialItems = new List<CartItem>
			{
				new CartItem { Id = 1, Name = "Product 1", Quantity = 1, Price = 10.0m }
			}
			};

			_cartRepositoryMock
				.Setup(r => r.GetCartByIdAsync(command.CartId))
				.ReturnsAsync(existingCart);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Single(existingCart.CartItems);
			_cartRepositoryMock.Verify(r => r.SaveCartAsync(existingCart), Times.Once);
		}
	}
}
