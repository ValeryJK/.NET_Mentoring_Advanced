using Cart.Application.Features.CreateCart;
using Cart.Domain.Entities;
using Cart.Domain.Interfaces;
using Moq;

namespace Cart.UnitTests.Command
{
    public class CreateCartCommandHandlerTests
    {
        private readonly CreateCartCommandHandler handler;
        private readonly Mock<ICartRepository> cartRepositoryMock;

        public CreateCartCommandHandlerTests()
        {
            this.cartRepositoryMock = new Mock<ICartRepository>();
            this.handler = new CreateCartCommandHandler(this.cartRepositoryMock.Object);
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

            this.cartRepositoryMock
                .Setup(r => r.GetCartByIdAsync(command.CartId))
                .ReturnsAsync(existingCart);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(existingCart.CartItems);
            this.cartRepositoryMock.Verify(r => r.SaveCartAsync(existingCart), Times.Once);
        }
    }
}
