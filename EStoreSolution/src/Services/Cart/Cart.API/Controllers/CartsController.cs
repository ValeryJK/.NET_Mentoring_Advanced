using Cart.Application.Interfaces;
using Cart.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
	[Route("carts")]
	[ApiController]
	public class CartsController : ControllerBase
	{
		private readonly ICartService _service;

		public CartsController(ICartService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCarts()
		{
			var carts = await _service.GetAllCartsAsync();
			return Ok(carts);
		}

		[HttpGet("{cartId}")]
		public async Task<IActionResult> GetCart(Guid cartId)
		{
			var cart = await _service.GetCartAsync(cartId);
			return cart == null ? NotFound() : Ok(cart);
		}

		[HttpPost("{cartId}/items")]
		public async Task<IActionResult> AddItem(Guid cartId, [FromBody] CartItem item)
		{
			await _service.AddItemAsync(cartId, item);
			return Ok();
		}

		[HttpDelete("{cartId}/items/{itemId}")]
		public async Task<IActionResult> RemoveItem(Guid cartId, int itemId)
		{
			await _service.RemoveItemAsync(cartId, itemId);
			return NoContent();
		}

		[HttpDelete("{cartId}")]
		public async Task<IActionResult> ClearCart(Guid cartId)
		{
			await _service.ClearCartAsync(cartId);
			return NoContent();
		}
	}
}
