namespace Cart.Application.Features.CreateCart
{
    public class CreateCartCommandResponse
    {
        public string? Id { get; set; }

        public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
    }

    public class CartItemResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? ImageAltText { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
