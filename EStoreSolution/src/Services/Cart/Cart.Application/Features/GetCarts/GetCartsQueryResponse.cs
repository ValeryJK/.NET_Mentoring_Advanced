namespace Cart.Application.Features.GetCarts
{
	public class GetCartsQueryResponse
	{
		public required string Id { get; set; }
		public List<CartItemResponse> CartItems { get; set; } = new();
	}

	public class CartItemResponse
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? ImageUrl { get; set; }
		public string? ImageAltText { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
