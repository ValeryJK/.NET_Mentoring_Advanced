using Cart.Application.Features.DeleteCart;
using Cart.Application.Validation;
using Cart.Domain.Interfaces;
using Moq;

namespace Cart.UnitTests.Command
{
	public class DeleteCartCommandHandlerTests
	{
		private readonly DeleteCartCommandHandler _handler;
		private readonly Mock<ICartRepository> _cartRepositoryMock;

		public DeleteCartCommandHandlerTests()
		{
			_cartRepositoryMock = new Mock<ICartRepository>();
			_handler = new DeleteCartCommandHandler(_cartRepositoryMock.Object);
		}

		[Fact]
		public async Task Handle_DeletesCart_WhenCartExists()
		{
			// Arrange
			var cartId = "test-cart-id";
			var cart = new Cart.Domain.Entities.Cart { Id = cartId };

			_cartRepositoryMock
				.Setup(r => r.GetCartByIdAsync(cartId))
				.ReturnsAsync(cart);

			// Act
			var result = await _handler.Handle(new DeleteCartCommand { CartId = cartId }, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			_cartRepositoryMock.Verify(r => r.DeleteCartAsync(cartId), Times.Once);
		}

		[Fact]
		public async Task Handle_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Arrange
			var cartId = "non-existent-cart-id";

			_cartRepositoryMock.Setup(r => r.GetCartByIdAsync(cartId));

			// Act
			var result = await _handler.Handle(new DeleteCartCommand { CartId = cartId }, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.IsType<NotFoundError>(result.Errors[0]);
			_cartRepositoryMock.Verify(r => r.DeleteCartAsync(It.IsAny<string>()), Times.Never);
		}
	}

}
