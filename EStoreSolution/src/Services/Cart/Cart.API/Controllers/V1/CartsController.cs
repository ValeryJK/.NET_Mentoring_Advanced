using Asp.Versioning;
using Cart.API.Extensions;
using Cart.Application.Features.CreateCart;
using Cart.Application.Features.DeleteCart;
using Cart.Application.Features.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers.V1
{
	/// <summary>
	/// Provides operations to manage shopping carts.
	/// </summary>
	[ApiVersion("1.0", Deprecated = false)]
	[Route("api/v{version:apiversion}/carts")]
	[ApiController]
	public class CartsController : ControllerBase
	{
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="CartsController"/> class.
		/// </summary>
		public CartsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Returns a cart model (cart key + list of cart items).
		/// </summary>
		/// <param name="cartId">The unique identifier of the cart.</param>
		/// <returns>The requested cart if found; otherwise, a 404 status code.</returns>
		/// <response code="200">Returns the requested cart.</response>
		/// <response code="404">If the cart is not found.</response>
		[HttpGet("{cartId}")]
		public async Task<IActionResult> GetCartInfo(string cartId)
		{
			var result = await _mediator.Send(new GetCartQuery { CartId = cartId });
			return result.ToHttpResponse();
		}

		/// <summary>
		/// Adds a new item to the specified cart.
		/// </summary>
		/// <returns>A 200 status code if the item was added successfully.</returns>
		[HttpPost("{cartId}/items")]
		public async Task<IActionResult> AddItem(CreateCartCommand command, string cartId)
		{
			var result = await _mediator.Send(new CreateCartCommand { CartId = cartId, InitialItems = command.InitialItems });
			return result.ToCreatedHttpResponse();
		}

		/// <summary>
		/// Removes an item from the specified cart.
		/// </summary>
		/// <param name="cartId">The unique identifier of the cart.</param>
		/// <param name="itemId">The unique identifier of the item to remove.</param>
		/// <returns>A 204 status code if the item was removed successfully.</returns>
		[HttpDelete("{cartId}/items/{itemId}")]
		public async Task<IActionResult> DeleteItem(string cartId, int itemId)
		{
			var command = new DeleteCartItemCommand { CartId = cartId, ItemId = itemId };
			var result = await _mediator.Send(command);
			return result.ToHttpResponse();
		}
	}
}