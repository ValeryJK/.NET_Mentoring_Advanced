﻿using Cart.Application.Validation;
using Cart.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Cart.Application.Features.DeleteCart
{
	public class DeleteCartCommand : IRequest<Result<Unit>>
	{
		public required string CartId { get; set; }
	}

	public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand, Result<Unit>>
	{
		private readonly ICartRepository _cartRepository;

		public DeleteCartCommandHandler(ICartRepository cartRepository)
		{
			_cartRepository = cartRepository;
		}

		public async Task<Result<Unit>> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
		{
			var cart = await _cartRepository.GetCartByIdAsync(request.CartId);
			if (cart is null)
			{
				return Result.Fail(new NotFoundError($"Cart with id {request.CartId} was not found."));
			}

			await _cartRepository.DeleteCartAsync(cart.Id);
			return Result.Ok();
		}
	}
}