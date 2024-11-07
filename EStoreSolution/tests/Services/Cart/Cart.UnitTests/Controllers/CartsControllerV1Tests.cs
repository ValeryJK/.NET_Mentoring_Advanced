using Cart.API.Controllers.V1;
using Cart.Application.Features.CreateCart;
using Cart.Application.Features.DeleteCart;
using Cart.Application.Features.GetCart;
using Cart.Application.Validation;
using Cart.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cart.UnitTests.Controllers
{
	public class CartsControllerV1Tests
	{
		private readonly CartsController _controller;
		private readonly Mock<IMediator> _mediatorMock;

		public CartsControllerV1Tests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new CartsController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetCartInfo_ReturnsOkResult_WhenCartExists()
		{
			// Arrange
			var cartId = "test-cart-id";
			var cartResponse = new GetCartQueryResponse
			{
				Id = cartId,
				CartItems = new List<Cart.Application.Features.GetCart.CartItemResponse>
			{
				new Cart.Application.Features.GetCart.CartItemResponse { Id = 1, Name = "Product 1", Quantity = 2, Price = 10.0m }
			}
			};
			var result = Result.Ok(cartResponse);

			_mediatorMock
				.Setup(m => m.Send(It.Is<GetCartQuery>(q => q.CartId == cartId), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.GetCartInfo(cartId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(actionResult);
			var returnValue = Assert.IsType<GetCartQueryResponse>(okResult.Value);
			Assert.Equal(cartId, returnValue.Id);
			Assert.Single(returnValue.CartItems);
		}

		[Fact]
		public async Task GetCartInfo_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Arrange
			var cartId = "non-existent-cart-id";
			var result = Result.Fail<GetCartQueryResponse>(new NotFoundError($"Cart with id {cartId} was not found."));

			_mediatorMock
				.Setup(m => m.Send(It.Is<GetCartQuery>(q => q.CartId == cartId), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.GetCartInfo(cartId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
			var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
			Assert.Equal($"Cart with id {cartId} was not found.", problemDetails.Detail);
		}

		[Fact]
		public async Task AddItem_ReturnsCreatedResult_WhenItemAddedSuccessfully()
		{
			// Arrange
			var cartId = "test-cart-id";
			var command = new CreateCartCommand
			{
				CartId = cartId,
				InitialItems = new List<CartItem>
		{
			new CartItem { Id = 1, Name = "Product 1", Quantity = 2, Price = 10.0m }
		}
			};
			var response = new CreateCartCommandResponse { Id = cartId };
			var result = Result.Ok(response);

			_mediatorMock
				.Setup(m => m.Send(It.IsAny<CreateCartCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.AddItem(command, cartId);

			// Assert
			var createdResult = Assert.IsType<ObjectResult>(actionResult);
			Assert.Equal(201, createdResult.StatusCode);
			var returnValue = Assert.IsType<CreateCartCommandResponse>(createdResult.Value);
			Assert.Equal(cartId, returnValue.Id);
		}

		[Fact]
		public async Task DeleteItem_ReturnsNoContent_WhenItemDeletedSuccessfully()
		{
			// Arrange
			var cartId = "test-cart-id";
			var itemId = 1;
			var result = Result.Ok(Unit.Value);

			_mediatorMock
				.Setup(m => m.Send(It.Is<DeleteCartItemCommand>(c => c.CartId == cartId && c.ItemId == itemId), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.DeleteItem(cartId, itemId);

			// Assert
			Assert.IsType<NoContentResult>(actionResult);
		}

		[Fact]
		public async Task DeleteItem_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Arrange
			var cartId = "non-existent-cart-id";
			var itemId = 1;
			var result = Result.Fail<Unit>(new NotFoundError($"Cart with id {cartId} was not found."));

			_mediatorMock
				.Setup(m => m.Send(It.Is<DeleteCartItemCommand>(c => c.CartId == cartId && c.ItemId == itemId), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.DeleteItem(cartId, itemId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
			var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
			Assert.Equal($"Cart with id {cartId} was not found.", problemDetails.Detail);
		}
	}
}