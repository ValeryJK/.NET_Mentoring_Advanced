using Cart.Application.Validation;
using Cart.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Cart.Application.Features.DeleteCart
{
	public class DeleteCartItemCommand : IRequest<Result<Unit>>
	{
		public required string CartId { get; set; }
		public int ItemId { get; set; }
	}

	public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result<Unit>>
	{
		private readonly ICartRepository _cartRepository;

		public DeleteCartItemCommandHandler(ICartRepository cartRepository)
		{
			_cartRepository = cartRepository;
		}

		public async Task<Result<Unit>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
		{
			var cart = await _cartRepository.GetCartByIdAsync(request.CartId);
			if (cart is null)
			{
				return Result.Fail(new NotFoundError($"Cart with id {request.CartId} was not found."));
			}

			cart.CartItems.RemoveAll(i => i.Id == request.ItemId);
			await _cartRepository.SaveCartAsync(cart);

			return Result.Ok(Unit.Value);
		}
	}
}