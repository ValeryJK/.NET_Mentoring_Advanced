using Cart.Domain.Entities;
using Cart.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Cart.Application.Features.CreateCart
{
    public class CreateCartCommand : IRequest<Result<CreateCartCommandResponse>>
    {
        public required string CartId { get; set; }

        public List<CartItem> InitialItems { get; set; } = new();
    }

    public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, Result<CreateCartCommandResponse>>
    {
        private readonly ICartRepository cartRepository;

        public CreateCartCommandHandler(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public async Task<Result<CreateCartCommandResponse>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await this.cartRepository.GetCartByIdAsync(request.CartId) ?? new Domain.Entities.Cart { Id = request.CartId };

            cart.CartItems.AddRange(request.InitialItems);
            await this.cartRepository.SaveCartAsync(cart);

            return Result.Ok(cart.Adapt<CreateCartCommandResponse>());
        }
    }
}