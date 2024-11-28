using Cart.Application.Validation;
using Cart.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Cart.Application.Features.GetCart
{
    public class GetCartQuery : IRequest<Result<GetCartQueryResponse>>
    {
        public required string CartId { get; set; }
    }

    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<GetCartQueryResponse>>
    {
        private readonly ICartRepository cartRepository;

        public GetCartQueryHandler(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public async Task<Result<GetCartQueryResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await this.cartRepository.GetCartByIdAsync(request.CartId);
            if (cart is null)
            {
                return Result.Fail(new NotFoundError($"Cart with id {request.CartId} was not found."));
            }

            var response = cart.Adapt<GetCartQueryResponse>();
            return Result.Ok(response);
        }
    }
}