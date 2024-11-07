using Cart.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Cart.Application.Features.GetCarts
{
	public class GetCartsQuery : IRequest<Result<List<GetCartsQueryResponse>>>
	{
	}

	public class GetCartsQueryHandler : IRequestHandler<GetCartsQuery, Result<List<GetCartsQueryResponse>>>
	{
		private readonly ICartRepository _cartRepository;

		public GetCartsQueryHandler(ICartRepository cartRepository)
		{
			_cartRepository = cartRepository;
		}

		public async Task<Result<List<GetCartsQueryResponse>>> Handle(GetCartsQuery request, CancellationToken cancellationToken)
		{
			var carts = await _cartRepository.GetAllCartsAsync();
			var response = carts.Adapt<List<GetCartsQueryResponse>>();

			return Result.Ok(response);
		}
	}
}