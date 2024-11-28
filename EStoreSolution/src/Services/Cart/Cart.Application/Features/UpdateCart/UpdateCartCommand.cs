using Cart.Application.Validation;
using Cart.Domain.Entities;
using Cart.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Cart.Application.Features.UpdateCart
{
    public class UpdateCartCommand : IRequest<Result<UpdateCartCommandResponse>>
    {
        public required string CartId { get; set; }

        public List<UpdateCartItem> CartItems { get; set; } = new();
    }

    public class UpdateCartItem
    {
        public int ItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }

    public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, Result<UpdateCartCommandResponse>>
    {
        private readonly ICartRepository cartRepository;

        public UpdateCartCommandHandler(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public async Task<Result<UpdateCartCommandResponse>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await this.cartRepository.GetCartByIdAsync(request.CartId);
            if (cart is null)
            {
                return Result.Fail(new NotFoundError($"Cart with id {request.CartId} was not found."));
            }

            foreach (var itemDto in request.CartItems)
            {
                var existingItem = cart.CartItems.Find(i => i.Id == itemDto.ItemId);
                if (existingItem is not null)
                {
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.Price = itemDto.Price;
                    existingItem.ImageUrl = itemDto.ImageUrl;
                    existingItem.Name = itemDto.Name;
                }
                else
                {
                    cart.CartItems.Add(itemDto.Adapt<CartItem>());
                }
            }

            await this.cartRepository.SaveCartAsync(cart);

            var response = cart.Adapt<UpdateCartCommandResponse>();
            return Result.Ok(response);
        }
    }
}