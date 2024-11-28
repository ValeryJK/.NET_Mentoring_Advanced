namespace Cart.Application.Features.UpdateCart
{
    public class UpdateCartCommandResponse
    {
        public string? CartId { get; set; }

        public List<UpdatedCartItemDto> CartItems { get; set; } = new();
    }

    public class UpdatedCartItemDto
    {
        public int ItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
