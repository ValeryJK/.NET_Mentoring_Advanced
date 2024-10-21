﻿namespace Cart.Domain.Entities
{
	public class CartItem
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? ImageUrl { get; set; }
		public string? ImageAltText { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
